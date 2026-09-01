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
    public class DesignsController : Controller
    {
        private readonly AppDbContext _context;

        public DesignsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DesignVersionStatus? status)
        {
            var query = _context.DesignVersions
                .Include(d => d.KitchenRequest)
                    .ThenInclude(r => r.Customer)
                .Include(d => d.Designer)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(d => d.Status == status.Value);

            var list = await query.OrderByDescending(d => d.CreatedAt).ToListAsync();
            ViewBag.Status = status;
            return View(list);
        }

        public async Task<IActionResult> CreateVersion(int requestId)
        {
            var req = await _context.KitchenRequests
                .Include(r => r.Customer)
                .Include(r => r.DesignVersions)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (req == null) return NotFound();

            int nextVersion = (req.DesignVersions.Any() ? req.DesignVersions.Max(d => d.VersionNumber) : 0) + 1;

            ViewBag.Request = req;
            ViewBag.Designers = await _context.StaffMembers.Where(s => s.Role == StaffRole.Designer && s.IsActive).ToListAsync();

            var model = new DesignVersion
            {
                KitchenRequestId = requestId,
                VersionNumber = nextVersion,
                SoftwareUsed = "SketchUp"
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVersion(DesignVersion model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.Now;
                model.CreatedBy = User.Identity?.Name ?? "Admin";
                model.Status = DesignVersionStatus.InternalReview;

                _context.DesignVersions.Add(model);

                var req = await _context.KitchenRequests.FindAsync(model.KitchenRequestId);
                if (req != null)
                {
                    req.Status = KitchenRequestStatus.AwaitingDesignApproval;
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = $"تم حفظ إصدار التصميم ({model.VersionCode}) بنجاح.";
                return RedirectToAction("Details", "Requests", new { id = model.KitchenRequestId });
            }

            ViewBag.Request = await _context.KitchenRequests.Include(r => r.Customer).FirstOrDefaultAsync(r => r.Id == model.KitchenRequestId);
            ViewBag.Designers = await _context.StaffMembers.Where(s => s.Role == StaffRole.Designer && s.IsActive).ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendToCustomer(int designId)
        {
            var design = await _context.DesignVersions.FindAsync(designId);
            if (design == null) return NotFound();

            design.Status = DesignVersionStatus.SentToCustomer;
            await _context.SaveChangesAsync();
            TempData["Success"] = "تم إرسال المخططات ثلاثية الأبعاد للعميل للاعتماد.";
            return RedirectToAction("Details", "Requests", new { id = design.KitchenRequestId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveByCustomer(int designId, string feedback)
        {
            var design = await _context.DesignVersions.Include(d => d.KitchenRequest).FirstOrDefaultAsync(d => d.Id == designId);
            if (design == null) return NotFound();

            design.Status = DesignVersionStatus.ApprovedByCustomer;
            design.IsLocked = true;
            design.CustomerApprovedAt = DateTime.Now;
            design.CustomerFeedback = feedback ?? "معتمد بدون ملاحظات";

            var otherVersions = await _context.DesignVersions
                .Where(d => d.KitchenRequestId == design.KitchenRequestId && d.Id != designId && !d.IsLocked)
                .ToListAsync();

            foreach (var v in otherVersions)
            {
                v.IsLocked = true;
            }

            var req = design.KitchenRequest;
            if (req != null)
            {
                req.Status = KitchenRequestStatus.InPricing;
                _context.RequestStatusHistories.Add(new RequestStatusHistory
                {
                    KitchenRequestId = req.Id,
                    OldStatus = KitchenRequestStatus.AwaitingDesignApproval,
                    NewStatus = KitchenRequestStatus.InPricing,
                    ChangedBy = User.Identity?.Name ?? "Admin",
                    Notes = $"تم اعتماد إصدار التصميم {design.VersionCode} رسمياً وقفل الإصدار والانتقال للتسعير."
                });
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"تم اعتماد التصميم {design.VersionCode} رسمياً وقفله، والمشروع جاهز للتسعير.";
            return RedirectToAction("Details", "Requests", new { id = design.KitchenRequestId });
        }
    }
}
