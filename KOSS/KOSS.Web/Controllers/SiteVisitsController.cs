using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KOSS.Web.Helpers;
using KOSS.Web.Models;

namespace KOSS.Web.Controllers
{
    [Authorize]
    public class SiteVisitsController : Controller
    {
        private readonly KossDbContext db = new KossDbContext();

        // ──────────────────────────────────────────────
        //  GET: /SiteVisits  -  قائمة المعاينات والزيارات
        // ──────────────────────────────────────────────
        public ActionResult Index(SiteVisitStatus? status)
        {
            var query = db.SiteVisits
                .Include(s => s.KitchenRequest)
                .Include(s => s.KitchenRequest.Customer)
                .Include(s => s.AssignedSurveyor)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(s => s.Status == status.Value);

            var list = query.OrderByDescending(s => s.ScheduledDate).ToList();
            ViewBag.Status = status;
            return View(list);
        }

        // ──────────────────────────────────────────────
        //  GET: /SiteVisits/Schedule?requestId=5
        // ──────────────────────────────────────────────
        public ActionResult Schedule(int? requestId)
        {
            if (requestId == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var request = db.KitchenRequests.Include(r => r.Customer).FirstOrDefault(r => r.Id == requestId.Value);
            if (request == null) return HttpNotFound();

            ViewBag.Request = request;
            ViewBag.Surveyors = new SelectList(db.StaffMembers.Where(s => s.Role == StaffRole.FieldSurveyor || s.Role == StaffRole.Designer).ToList(), "Id", "FullName");

            return View(new SiteVisit
            {
                KitchenRequestId = request.Id,
                ScheduledDate = DateTime.Now.AddDays(1)
            });
        }

        // ──────────────────────────────────────────────
        //  POST: /SiteVisits/Schedule
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Schedule(SiteVisit visit)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Surveyors = new SelectList(db.StaffMembers.Where(s => s.Role == StaffRole.FieldSurveyor || s.Role == StaffRole.Designer).ToList(), "Id", "FullName", visit.AssignedSurveyorId);
                return View(visit);
            }

            visit.Status = SiteVisitStatus.Scheduled;
            visit.CreatedAt = DateTime.Now;
            db.SiteVisits.Add(visit);

            // تحديث حالة الطلب إلى بانتظار المعاينة
            var request = db.KitchenRequests.Find(visit.KitchenRequestId);
            if (request != null && request.Status < KitchenRequestStatus.AwaitingSiteVisit)
            {
                RequestWorkflowEngine.Transition(db, request, KitchenRequestStatus.AwaitingSiteVisit, User.Identity.GetUserName(), "جدولة موعد المعاينة الميدانية.");
            }

            db.SaveChanges();
            TempData["Success"] = "تم جدولة موعد المعاينة الميدانية بنجاح!";
            return RedirectToAction("Details", "Requests", new { id = visit.KitchenRequestId });
        }

        // ──────────────────────────────────────────────
        //  GET: /SiteVisits/RecordMeasurements/5
        // ──────────────────────────────────────────────
        public ActionResult RecordMeasurements(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var visit = db.SiteVisits
                .Include(s => s.KitchenRequest)
                .Include(s => s.KitchenRequest.Customer)
                .FirstOrDefault(s => s.Id == id.Value);

            if (visit == null) return HttpNotFound();
            return View(visit);
        }

        // ──────────────────────────────────────────────
        //  POST: /SiteVisits/RecordMeasurements
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RecordMeasurements(SiteVisit model)
        {
            var visit = db.SiteVisits.Find(model.Id);
            if (visit == null) return HttpNotFound();

            visit.WallLength1Cm = model.WallLength1Cm;
            visit.WallLength2Cm = model.WallLength2Cm;
            visit.WallLength3Cm = model.WallLength3Cm;
            visit.CeilingHeightCm = model.CeilingHeightCm;
            visit.EstimatedAreaM2 = model.EstimatedAreaM2;
            visit.PlumbingNotes = model.PlumbingNotes;
            visit.ElectricalNotes = model.ElectricalNotes;
            visit.ObstaclesNotes = model.ObstaclesNotes;
            visit.SurveyorReport = model.SurveyorReport;
            visit.AttachmentsPath = model.AttachmentsPath;
            visit.ActualVisitDate = DateTime.Now;
            visit.Status = SiteVisitStatus.AwaitingReview;

            db.SaveChanges();
            TempData["Success"] = "تم حفظ القياسات الميدانية وإرسال التقرير للمراجعة والاعتماد.";
            return RedirectToAction("Details", "Requests", new { id = visit.KitchenRequestId });
        }

        // ──────────────────────────────────────────────
        //  POST: /SiteVisits/Approve  -  اعتماد المعاينة والانتقال للتصميم
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Approve(int visitId)
        {
            var visit = db.SiteVisits.Include(s => s.KitchenRequest).FirstOrDefault(s => s.Id == visitId);
            if (visit == null) return HttpNotFound();

            visit.Status = SiteVisitStatus.Approved;
            visit.ApprovedBy = User.Identity.GetUserName();
            visit.ApprovedAt = DateTime.Now;

            // تحديث حالة الطلب إلى تمت المعاينة
            var request = visit.KitchenRequest;
            if (request != null && request.Status <= KitchenRequestStatus.SiteVisitCompleted)
            {
                RequestWorkflowEngine.Transition(db, request, KitchenRequestStatus.SiteVisitCompleted, User.Identity.GetUserName(), "اعتماد القياسات والمعاينة الميدانية رسمياً.");
            }

            db.SaveChanges();
            TempData["Success"] = "تم اعتماد المعاينة والقياسات بنجاح. الطلب مؤهل الآن لمرحلة التصميم.";
            return RedirectToAction("Details", "Requests", new { id = visit.KitchenRequestId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
