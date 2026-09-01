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
    public class SiteVisitsController : Controller
    {
        private readonly AppDbContext _context;

        public SiteVisitsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(SiteVisitStatus? status)
        {
            var query = _context.SiteVisits
                .Include(s => s.KitchenRequest)
                    .ThenInclude(r => r.Customer)
                .Include(s => s.AssignedSurveyor)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(s => s.Status == status.Value);

            var list = await query.OrderByDescending(s => s.ScheduledDate).ToListAsync();
            ViewBag.Status = status;
            return View(list);
        }

        public async Task<IActionResult> Schedule(int requestId)
        {
            var req = await _context.KitchenRequests.Include(r => r.Customer).FirstOrDefaultAsync(r => r.Id == requestId);
            if (req == null) return NotFound();

            ViewBag.Request = req;
            ViewBag.Surveyors = await _context.StaffMembers.Where(s => s.Role == StaffRole.FieldSurveyor && s.IsActive).ToListAsync();

            var model = new SiteVisit
            {
                KitchenRequestId = requestId,
                ScheduledDate = DateTime.Now.AddDays(1),
                Status = SiteVisitStatus.Scheduled
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Schedule(SiteVisit model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.Now;
                model.Status = SiteVisitStatus.Scheduled;
                _context.SiteVisits.Add(model);

                var req = await _context.KitchenRequests.FindAsync(model.KitchenRequestId);
                if (req != null && req.Status == KitchenRequestStatus.AwaitingSiteVisit)
                {
                    req.Status = KitchenRequestStatus.AwaitingSiteVisit;
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "تمت جدولة موعد المعاينة وتكليف المهندس بنجاح.";
                return RedirectToAction("Details", "Requests", new { id = model.KitchenRequestId });
            }

            ViewBag.Request = await _context.KitchenRequests.Include(r => r.Customer).FirstOrDefaultAsync(r => r.Id == model.KitchenRequestId);
            ViewBag.Surveyors = await _context.StaffMembers.Where(s => s.Role == StaffRole.FieldSurveyor && s.IsActive).ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> RecordMeasurements(int id)
        {
            var visit = await _context.SiteVisits
                .Include(s => s.KitchenRequest)
                    .ThenInclude(r => r.Customer)
                .Include(s => s.AssignedSurveyor)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (visit == null) return NotFound();
            return View(visit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordMeasurements(SiteVisit model)
        {
            var visit = await _context.SiteVisits.FindAsync(model.Id);
            if (visit == null) return NotFound();

            visit.WallLength1Cm = model.WallLength1Cm;
            visit.WallLength2Cm = model.WallLength2Cm;
            visit.WallLength3Cm = model.WallLength3Cm;
            visit.CeilingHeightCm = model.CeilingHeightCm;
            visit.EstimatedAreaM2 = model.EstimatedAreaM2;
            visit.PlumbingNotes = model.PlumbingNotes;
            visit.ElectricalNotes = model.ElectricalNotes;
            visit.ObstaclesNotes = model.ObstaclesNotes;
            visit.SurveyorReport = model.SurveyorReport;
            visit.ActualVisitDate = DateTime.Now;
            visit.Status = SiteVisitStatus.AwaitingReview;

            await _context.SaveChangesAsync();
            TempData["Success"] = "تم حفظ القياسات والتقرير الفني بنجاح، والمعاينة بانتظار الاعتماد.";
            return RedirectToAction("Details", "Requests", new { id = visit.KitchenRequestId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int visitId)
        {
            var visit = await _context.SiteVisits.Include(v => v.KitchenRequest).FirstOrDefaultAsync(v => v.Id == visitId);
            if (visit == null) return NotFound();

            visit.Status = SiteVisitStatus.Approved;
            visit.ApprovedBy = User.Identity?.Name ?? "Admin";
            visit.ApprovedAt = DateTime.Now;

            var req = visit.KitchenRequest;
            if (req != null)
            {
                req.Status = KitchenRequestStatus.InDesign;
                _context.RequestStatusHistories.Add(new RequestStatusHistory
                {
                    KitchenRequestId = req.Id,
                    OldStatus = KitchenRequestStatus.SiteVisitCompleted,
                    NewStatus = KitchenRequestStatus.InDesign,
                    ChangedBy = User.Identity?.Name ?? "Admin",
                    Notes = "تم اعتماد القياسات الميدانية رسمياً وتحويل الطلب للمصمم لبدء المخططات ثلاثية الأبعاد."
                });
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "تم اعتماد المعاينة والقياسات بنجاح، وتحويل المشروع لمرحلة التصميم.";
            return RedirectToAction("Details", "Requests", new { id = visit.KitchenRequestId });
        }
    }
}
