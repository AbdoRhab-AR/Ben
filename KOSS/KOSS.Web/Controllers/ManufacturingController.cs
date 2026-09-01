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
    public class ManufacturingController : Controller
    {
        private readonly AppDbContext _context;

        public ManufacturingController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.WorkOrders
                .Include(w => w.KitchenRequest)
                    .ThenInclude(r => r.Customer)
                .Include(w => w.Tasks)
                .Include(w => w.QualityChecks)
                .Where(w => w.Status == WorkOrderStatus.Manufacturing || w.Status == WorkOrderStatus.Planning)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteTask(int taskId, int workOrderId)
        {
            var task = await _context.ManufacturingTasks.FindAsync(taskId);
            if (task == null) return NotFound();

            task.Status = "Completed";
            task.CompletedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["Success"] = $"تم اكتمال مرحلة ({task.TaskName}) بنجاح.";
            return RedirectToAction("Details", "WorkOrders", new { id = workOrderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PerformQualityCheck(int workOrderId, bool isPassed, string notes)
        {
            var wo = await _context.WorkOrders.Include(w => w.KitchenRequest).FirstOrDefaultAsync(w => w.Id == workOrderId);
            if (wo == null) return NotFound();

            var qc = new QualityCheck
            {
                WorkOrderId = workOrderId,
                ReportNumber = $"QC-{DateTime.Now.Year}-{new Random().Next(1000, 9999)}",
                InspectionDate = DateTime.Now,
                InspectorName = User.Identity?.Name ?? "Quality Manager",
                Passed = isPassed,
                Notes = notes
            };

            _context.QualityChecks.Add(qc);

            if (isPassed)
            {
                wo.Status = WorkOrderStatus.Completed;
                wo.ActualEndDate = DateTime.Now;

                var req = wo.KitchenRequest;
                if (req != null)
                {
                    req.Status = KitchenRequestStatus.ReadyForInstallation;
                    _context.RequestStatusHistories.Add(new RequestStatusHistory
                    {
                        KitchenRequestId = req.Id,
                        OldStatus = KitchenRequestStatus.InManufacturing,
                        NewStatus = KitchenRequestStatus.ReadyForInstallation,
                        ChangedBy = User.Identity?.Name ?? "Admin",
                        Notes = "اجتياز فحص الجودة والمطابقة بالمصنع بنجاح. المطبخ جاهز للنقل والتركيب."
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = isPassed ? "تم اعتماد تقرير فحص الجودة بنجاح، والمشروع جاهز للتركيب." : "تم تسجيل ملاحظات الفحص ويلزم معالجتها قبل الشحن.";
            return RedirectToAction("Details", "WorkOrders", new { id = workOrderId });
        }
    }
}
