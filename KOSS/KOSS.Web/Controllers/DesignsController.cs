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
    public class DesignsController : Controller
    {
        private readonly KossDbContext db = new KossDbContext();

        // ──────────────────────────────────────────────
        //  GET: /Designs  -  قائمة التصاميم ومراحلها
        // ──────────────────────────────────────────────
        public ActionResult Index(DesignVersionStatus? status)
        {
            var query = db.DesignVersions
                .Include(d => d.KitchenRequest)
                .Include(d => d.KitchenRequest.Customer)
                .Include(d => d.Designer)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(d => d.Status == status.Value);

            var list = query.OrderByDescending(d => d.CreatedAt).ToList();
            ViewBag.Status = status;
            return View(list);
        }

        // ──────────────────────────────────────────────
        //  GET: /Designs/CreateVersion?requestId=5
        // ──────────────────────────────────────────────
        public ActionResult CreateVersion(int? requestId)
        {
            if (requestId == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var request = db.KitchenRequests
                .Include(r => r.Customer)
                .Include(r => r.SiteVisits)
                .FirstOrDefault(r => r.Id == requestId.Value);

            if (request == null) return HttpNotFound();

            // فحص إلزامي: هل المعاينة معتمدة؟
            if (!request.SiteVisits.Any(s => s.Status == SiteVisitStatus.Approved))
            {
                TempData["Error"] = "لا يمكن فتح مهمة تصميم إلا بعد اعتماد المعاينة الميدانية والقياسات.";
                return RedirectToAction("Details", "Requests", new { id = requestId });
            }

            int currentVersionsCount = db.DesignVersions.Count(d => d.KitchenRequestId == requestId.Value);
            int nextVer = currentVersionsCount + 1;

            ViewBag.Request = request;
            ViewBag.Designers = new SelectList(db.StaffMembers.Where(s => s.Role == StaffRole.Designer).ToList(), "Id", "FullName");

            return View(new DesignVersion
            {
                KitchenRequestId = request.Id,
                VersionNumber = nextVer,
                SoftwareUsed = "SketchUp"
            });
        }

        // ──────────────────────────────────────────────
        //  POST: /Designs/CreateVersion
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CreateVersion(DesignVersion version)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Designers = new SelectList(db.StaffMembers.Where(s => s.Role == StaffRole.Designer).ToList(), "Id", "FullName", version.DesignerId);
                return View(version);
            }

            version.Status = DesignVersionStatus.InternalReview;
            version.CreatedAt = DateTime.Now;
            version.CreatedBy = User.Identity.GetUserName();

            db.DesignVersions.Add(version);

            // تحديث حالة الطلب
            var request = db.KitchenRequests.Find(version.KitchenRequestId);
            if (request != null && request.Status < KitchenRequestStatus.InDesign)
            {
                RequestWorkflowEngine.Transition(db, request, KitchenRequestStatus.InDesign, User.Identity.GetUserName(), $"إعداد إصدار التصميم {version.VersionCode}");
            }

            db.SaveChanges();
            TempData["Success"] = $"تم حفظ إصدار التصميم {version.VersionCode} بنجاح وإرساله للمراجعة.";
            return RedirectToAction("Details", "Requests", new { id = version.KitchenRequestId });
        }

        // ──────────────────────────────────────────────
        //  POST: /Designs/SendToCustomer  -  إرسال التصميم للعميل للاعتماد
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SendToCustomer(int versionId)
        {
            var version = db.DesignVersions.Include(d => d.KitchenRequest).FirstOrDefault(d => d.Id == versionId);
            if (version == null) return HttpNotFound();

            version.Status = DesignVersionStatus.SentToCustomer;

            var request = version.KitchenRequest;
            if (request != null)
            {
                RequestWorkflowEngine.Transition(db, request, KitchenRequestStatus.AwaitingDesignApproval, User.Identity.GetUserName(), $"إرسال إصدار التصميم {version.VersionCode} للعميل للاعتماد.");
            }

            db.SaveChanges();
            TempData["Success"] = $"تم إرسال التصميم {version.VersionCode} للعميل بنجاح وبانتظار موافقته.";
            return RedirectToAction("Details", "Requests", new { id = version.KitchenRequestId });
        }

        // ──────────────────────────────────────────────
        //  POST: /Designs/ApproveByCustomer  -  اعتماد العميل للتصميم وقفل الإصدار
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApproveByCustomer(int versionId, string feedback)
        {
            var version = db.DesignVersions.Include(d => d.KitchenRequest).FirstOrDefault(d => d.Id == versionId);
            if (version == null) return HttpNotFound();

            version.Status = DesignVersionStatus.ApprovedByCustomer;
            version.IsLocked = true; // قفل الإصدار المعتمد منعاً للتعديل المباشر
            version.CustomerApprovedAt = DateTime.Now;
            version.CustomerFeedback = feedback;

            var request = version.KitchenRequest;
            if (request != null)
            {
                RequestWorkflowEngine.Transition(db, request, KitchenRequestStatus.InPricing, User.Identity.GetUserName(), $"اعتماد العميل للتصميم {version.VersionCode} رسمياً. جاهز للتسعير.");
            }

            db.SaveChanges();
            TempData["Success"] = $"تم اعتماد التصميم {version.VersionCode} وقفل الإصدار. الطلب جاهز الآن لإعداد عرض السعر.";
            return RedirectToAction("Details", "Requests", new { id = version.KitchenRequestId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
