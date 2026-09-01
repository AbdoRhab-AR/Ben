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
    public class WorkOrdersController : Controller
    {
        private readonly AppDbContext _context;

        public WorkOrdersController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(WorkOrderStatus? status)
        {
            var query = _context.WorkOrders
                .Include(w => w.KitchenRequest)
                    .ThenInclude(r => r.Customer)
                .Include(w => w.MaterialRequirements)
                .Include(w => w.Tasks)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(w => w.Status == status.Value);

            var list = await query.OrderByDescending(w => w.CreatedAt).ToListAsync();
            ViewBag.Status = status;
            return View(list);
        }

        public async Task<IActionResult> Details(int id)
        {
            var wo = await _context.WorkOrders
                .Include(w => w.KitchenRequest)
                    .ThenInclude(r => r.Customer)
                .Include(w => w.MaterialRequirements)
                .Include(w => w.Tasks)
                .Include(w => w.QualityChecks)
                .Include(w => w.InstallationOrders)
                .Include(w => w.HandoverDocuments)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (wo == null) return NotFound();
            return View(wo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartProduction(int workOrderId)
        {
            var wo = await _context.WorkOrders.Include(w => w.KitchenRequest).FirstOrDefaultAsync(w => w.Id == workOrderId);
            if (wo == null) return NotFound();

            wo.Status = WorkOrderStatus.Manufacturing;

            var req = wo.KitchenRequest;
            if (req != null)
            {
                req.Status = KitchenRequestStatus.InManufacturing;
                _context.RequestStatusHistories.Add(new RequestStatusHistory
                {
                    KitchenRequestId = req.Id,
                    OldStatus = KitchenRequestStatus.InPlanning,
                    NewStatus = KitchenRequestStatus.InManufacturing,
                    ChangedBy = User.Identity?.Name ?? "Admin",
                    Notes = $"بدء تصنيع وقص خامات المطبخ بالمصنع بموجب أمر التشغيل رقم {wo.OrderNumber}."
                });
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "تم بدء مرحلة التصنيع بالمصنع بنجاح.";
            return RedirectToAction(nameof(Details), new { id = workOrderId });
        }
    }
}
