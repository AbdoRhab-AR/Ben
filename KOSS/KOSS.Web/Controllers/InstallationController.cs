using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KOSS.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KOSS.Web.Controllers
{
    [Authorize]
    public class InstallationController : Controller
    {
        private readonly AppDbContext _context;

        public InstallationController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.InstallationOrders
                .Include(i => i.WorkOrder)
                    .ThenInclude(w => w.KitchenRequest)
                        .ThenInclude(r => r.Customer)
                .OrderByDescending(i => i.ScheduledDate)
                .ToListAsync();

            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Schedule(int workOrderId, DateTime scheduledDate, string teamLeadName, string vehicleNumber, string notes)
        {
            var wo = await _context.WorkOrders.Include(w => w.KitchenRequest).FirstOrDefaultAsync(w => w.Id == workOrderId);
            if (wo == null) return NotFound();

            var order = new InstallationOrder
            {
                WorkOrderId = workOrderId,
                OrderNumber = $"INST-{DateTime.Now.Year}-{new Random().Next(1000, 9999)}",
                ScheduledDate = scheduledDate,
                TeamLeadName = string.IsNullOrEmpty(teamLeadName) ? "فريق التركيبات الرئيسي" : teamLeadName,
                VehicleNumber = vehicleNumber ?? "شاحنة 1",
                InstallationReport = notes,
                Status = "Scheduled"
            };

            _context.InstallationOrders.Add(order);

            var req = wo.KitchenRequest;
            if (req != null)
            {
                req.Status = KitchenRequestStatus.InstallationScheduled;
                _context.RequestStatusHistories.Add(new RequestStatusHistory
                {
                    KitchenRequestId = req.Id,
                    OldStatus = KitchenRequestStatus.ReadyForInstallation,
                    NewStatus = KitchenRequestStatus.InstallationScheduled,
                    ChangedBy = User.Identity?.Name ?? "Admin",
                    Notes = $"جدولة موعد التركيب الميداني بتاريخ {scheduledDate:yyyy/MM/dd}."
                });
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "تمت جدولة موعد التركيب بنجاح.";
            return RedirectToAction("Details", "Requests", new { id = wo.KitchenRequestId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteHandover(int workOrderId, string clientNotes, bool customerAccepted, string customerSignerName)
        {
            var wo = await _context.WorkOrders.Include(w => w.KitchenRequest).FirstOrDefaultAsync(w => w.Id == workOrderId);
            if (wo == null) return NotFound();

            var handover = new HandoverDocument
            {
                KitchenRequestId = wo.KitchenRequestId,
                WorkOrderId = workOrderId,
                DocumentNumber = $"HND-{DateTime.Now.Year}-{new Random().Next(1000, 9999)}",
                HandoverDate = DateTime.Now,
                CustomerAccepted = customerAccepted,
                CustomerRemarks = clientNotes ?? "تم الاستلام والمطابقة بنجاح",
                CustomerSignerName = string.IsNullOrEmpty(customerSignerName) ? (wo.KitchenRequest?.Customer?.Name ?? "العميل") : customerSignerName,
                CompanyRepresentative = User.Identity?.Name ?? "مهندس الاستلام"
            };

            _context.HandoverDocuments.Add(handover);

            var req = wo.KitchenRequest;
            if (req != null)
            {
                if (customerAccepted)
                {
                    req.Status = KitchenRequestStatus.HandoverCompleted;
                    _context.RequestStatusHistories.Add(new RequestStatusHistory
                    {
                        KitchenRequestId = req.Id,
                        OldStatus = KitchenRequestStatus.InInstallation,
                        NewStatus = KitchenRequestStatus.HandoverCompleted,
                        ChangedBy = User.Identity?.Name ?? "Admin",
                        Notes = $"توقيع محضر التسليم الرسمي رقم {handover.DocumentNumber} بنجاح وقبول العميل للأعمال."
                    });
                }
                else
                {
                    req.Status = KitchenRequestStatus.AwaitingSnagResolution;
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = customerAccepted ? "تم اعتماد محضر التسليم النهائي بنجاح، والمشروع جاهز لتحصيل المخالصة والإغلاق." : "تم تسجيل ملاحظات العميل للمعالجة الفورية.";
            return RedirectToAction("Details", "Requests", new { id = wo.KitchenRequestId });
        }
    }
}
