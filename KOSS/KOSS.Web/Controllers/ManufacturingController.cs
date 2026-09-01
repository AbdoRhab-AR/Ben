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
    public class ManufacturingController : Controller
    {
        private readonly KossDbContext db = new KossDbContext();

        // ──────────────────────────────────────────────
        //  GET: /Manufacturing  -  لوحة المصنع ومتابعة مراحل الإنتاج
        // ──────────────────────────────────────────────
        public ActionResult Index()
        {
            var activeOrders = db.WorkOrders
                .Include(w => w.KitchenRequest)
                .Include(w => w.KitchenRequest.Customer)
                .Include(w => w.Tasks)
                .Include(w => w.QualityChecks.Select(qc => qc.SnagItems))
                .Where(w => w.Status == WorkOrderStatus.Manufacturing || w.Status == WorkOrderStatus.QualityInspection)
                .OrderBy(w => w.ExpectedEndDate)
                .ToList();

            return View(activeOrders);
        }

        // ──────────────────────────────────────────────
        //  POST: /Manufacturing/CompleteTask  -  إنجاز مرحلة تصنيع
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CompleteTask(int taskId, string technicianName, string notes)
        {
            var task = db.ManufacturingTasks.Include(t => t.WorkOrder).FirstOrDefault(t => t.Id == taskId);
            if (task == null) return HttpNotFound();

            task.Status = "Completed";
            task.TechnicianName = technicianName ?? User.Identity.GetUserName();
            task.CompletedAt = DateTime.Now;
            task.Notes = notes;

            // إذا اكتملت جميع المهام، يتم الانتقال إلى مرحلة فحص الجودة
            var wo = task.WorkOrder;
            if (wo != null && wo.Tasks.All(t => t.Status == "Completed"))
            {
                wo.Status = WorkOrderStatus.QualityInspection;
            }

            db.SaveChanges();
            TempData["Success"] = $"تم تسجيل إنجاز مرحلة [{task.TaskName}] بنجاح.";
            return RedirectToAction("Details", "WorkOrders", new { id = task.WorkOrderId });
        }

        // ──────────────────────────────────────────────
        //  POST: /Manufacturing/PerformQualityCheck  -  تنفيذ فحص الجودة والمطابقة
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PerformQualityCheck(int workOrderId, bool dimensionsMatched, bool surfacesFlawless, bool hardwareSmooth, bool packagingSecured, string notes, string snagDescription)
        {
            var wo = db.WorkOrders.Include(w => w.KitchenRequest).FirstOrDefault(w => w.Id == workOrderId);
            if (wo == null) return HttpNotFound();

            bool allPassed = dimensionsMatched && surfacesFlawless && hardwareSmooth && packagingSecured && string.IsNullOrEmpty(snagDescription);

            int lastQcId = db.QualityChecks.Any() ? db.QualityChecks.Max(q => q.Id) : 0;
            string reportNo = $"QC-{DateTime.Now.Year}-{(lastQcId + 1):D5}";

            var qc = new QualityCheck
            {
                WorkOrderId = workOrderId,
                ReportNumber = reportNo,
                InspectionDate = DateTime.Now,
                InspectorName = User.Identity.GetUserName(),
                DimensionsMatched = dimensionsMatched,
                SurfacesFlawless = surfacesFlawless,
                HardwareWorkingSmoothly = hardwareSmooth,
                PackagingSecured = packagingSecured,
                Passed = allPassed,
                Notes = notes
            };

            // تسجيل ملاحظة/نقص إن وجد
            if (!string.IsNullOrEmpty(snagDescription))
            {
                qc.SnagItems.Add(new SnagItem
                {
                    KitchenRequestId = wo.KitchenRequestId,
                    Description = snagDescription,
                    AssignedTo = "مشرف المصنع",
                    LoggedAt = DateTime.Now,
                    IsResolved = false
                });
            }

            db.QualityChecks.Add(qc);

            var request = wo.KitchenRequest;
            if (allPassed)
            {
                wo.Status = WorkOrderStatus.ReadyForInstallation;
                if (request != null)
                {
                    RequestWorkflowEngine.Transition(db, request, KitchenRequestStatus.ReadyForInstallation, User.Identity.GetUserName(), $"اجتياز فحص الجودة والمطابقة بتقرير #{reportNo}. المطبخ جاهز للنقل والتركيب.");
                }
                TempData["Success"] = $"تم اجتياز فحص الجودة بنجاح! تقرير #{reportNo} — المطبخ جاهز للجدولة والتركيب.";
            }
            else
            {
                wo.Status = WorkOrderStatus.SnagResolution;
                if (request != null)
                {
                    RequestWorkflowEngine.Transition(db, request, KitchenRequestStatus.AwaitingSnagResolution, User.Identity.GetUserName(), $"فحص الجودة #{reportNo} رصد ملاحظات تحتاج معالجة بالمصنع.");
                }
                TempData["Warning"] = $"تم تسجيل تقرير فحص الجودة #{reportNo} ورصد ملاحظات تحتاج معالجة قبل السماح بالتركيب.";
            }

            db.SaveChanges();
            return RedirectToAction("Details", "WorkOrders", new { id = workOrderId });
        }

        // ──────────────────────────────────────────────
        //  POST: /Manufacturing/ResolveSnag  -  معالجة ملاحظة / عيب
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ResolveSnag(int snagId)
        {
            var snag = db.SnagItems.Include(s => s.KitchenRequest).FirstOrDefault(s => s.Id == snagId);
            if (snag == null) return HttpNotFound();

            snag.IsResolved = true;
            snag.ResolvedAt = DateTime.Now;

            db.SaveChanges();
            TempData["Success"] = "تم تسجيل معالجة وإغلاق الملاحظة بنجاح.";
            return RedirectToAction("Details", "Requests", new { id = snag.KitchenRequestId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
