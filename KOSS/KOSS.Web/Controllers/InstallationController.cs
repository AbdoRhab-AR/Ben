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
    public class InstallationController : Controller
    {
        private readonly KossDbContext db = new KossDbContext();

        // ──────────────────────────────────────────────
        //  GET: /Installation  -  لوحة التركيبات والمواعيد
        // ──────────────────────────────────────────────
        public ActionResult Index()
        {
            var installations = db.InstallationOrders
                .Include(i => i.WorkOrder)
                .Include(i => i.WorkOrder.KitchenRequest)
                .Include(i => i.WorkOrder.KitchenRequest.Customer)
                .OrderBy(i => i.ScheduledDate)
                .ToList();

            return View(installations);
        }

        // ──────────────────────────────────────────────
        //  POST: /Installation/Schedule  -  جدولة موعد تركيب وتعيين الفريق
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Schedule(int workOrderId, DateTime scheduledDate, string teamLeadName, string vehicleNumber)
        {
            var wo = db.WorkOrders.Include(w => w.KitchenRequest).FirstOrDefault(w => w.Id == workOrderId);
            if (wo == null) return HttpNotFound();

            int lastInstId = db.InstallationOrders.Any() ? db.InstallationOrders.Max(i => i.Id) : 0;
            string instNo = $"INST-{DateTime.Now.Year}-{(lastInstId + 1):D5}";

            var inst = new InstallationOrder
            {
                WorkOrderId = workOrderId,
                OrderNumber = instNo,
                ScheduledDate = scheduledDate,
                TeamLeadName = teamLeadName ?? "فريق التركيب الرئيسي",
                VehicleNumber = vehicleNumber,
                Status = "Scheduled"
            };

            db.InstallationOrders.Add(inst);
            wo.Status = WorkOrderStatus.ReadyForInstallation;

            var request = wo.KitchenRequest;
            if (request != null)
            {
                RequestWorkflowEngine.Transition(db, request, KitchenRequestStatus.InstallationScheduled, User.Identity.GetUserName(), $"جدولة موعد التركيب بتاريخ {scheduledDate:yyyy/MM/dd} بقيادة الفني {teamLeadName}.");
            }

            db.SaveChanges();
            TempData["Success"] = $"تم جدولة أمر التركيب #{instNo} بنجاح بتاريخ {scheduledDate:yyyy/MM/dd}.";
            return RedirectToAction("Details", "WorkOrders", new { id = workOrderId });
        }

        // ──────────────────────────────────────────────
        //  POST: /Installation/StartInstallation  -  بدء أعمال التركيب الميداني
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StartInstallation(int installationOrderId)
        {
            var inst = db.InstallationOrders.Include(i => i.WorkOrder.KitchenRequest).FirstOrDefault(i => i.Id == installationOrderId);
            if (inst == null) return HttpNotFound();

            inst.Status = "InProgress";
            inst.WorkOrder.Status = WorkOrderStatus.Installing;

            var request = inst.WorkOrder.KitchenRequest;
            if (request != null)
            {
                RequestWorkflowEngine.Transition(db, request, KitchenRequestStatus.InInstallation, User.Identity.GetUserName(), "بدء أعمال التركيب الميداني في موقع العميل.");
            }

            db.SaveChanges();
            TempData["Success"] = "تم تسجيل بدء أعمال التركيب الميداني.";
            return RedirectToAction("Details", "WorkOrders", new { id = inst.WorkOrderId });
        }

        // ──────────────────────────────────────────────
        //  POST: /Installation/CompleteHandover  -  تحرير محضر التسليم الرسمي وتوقيع العميل
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CompleteHandover(int workOrderId, string customerSignerName, string companyRepresentative = "", string customerRemarks = "", bool customerAccepted = true, decimal installedLinearMeters = 0)
        {
            var wo = db.WorkOrders.Include(w => w.KitchenRequest).Include(w => w.Contract).Include(w => w.InstallationOrders).FirstOrDefault(w => w.Id == workOrderId);
            if (wo == null) return HttpNotFound();

            int lastDocId = db.HandoverDocuments.Any() ? db.HandoverDocuments.Max(h => h.Id) : 0;
            string docNo = $"HND-{DateTime.Now.Year}-{(lastDocId + 1):D5}";

            var handover = new HandoverDocument
            {
                KitchenRequestId = wo.KitchenRequestId,
                WorkOrderId = workOrderId,
                DocumentNumber = docNo,
                HandoverDate = DateTime.Now,
                CompanyRepresentative = companyRepresentative ?? User.Identity.GetUserName(),
                CustomerSignerName = customerSignerName,
                CustomerAccepted = customerAccepted,
                CustomerRemarks = customerRemarks
            };

            db.HandoverDocuments.Add(handover);

            // تحديث أمتار التركيب
            var latestInst = wo.InstallationOrders.OrderByDescending(i => i.Id).FirstOrDefault();
            if (latestInst != null)
            {
                latestInst.InstalledLinearMeters = installedLinearMeters > 0 ? installedLinearMeters : 10m;
                latestInst.Status = "FullyCompleted";
            }

            wo.Status = WorkOrderStatus.Completed;
            wo.ActualEndDate = DateTime.Now;

            var request = wo.KitchenRequest;
            var contract = wo.Contract ?? request.ActiveContract;

            if (request != null)
            {
                RequestWorkflowEngine.Transition(db, request, KitchenRequestStatus.HandoverCompleted, User.Identity.GetUserName(), $"توقيع محضر التسليم الرسمي #{docNo} واستلام العميل للعمل.");

                // إذا كان هناك رصيد متبقٍ غير مدفوع
                if (contract != null && contract.RemainingBalance > 0.01m)
                {
                    RequestWorkflowEngine.Transition(db, request, KitchenRequestStatus.AwaitingFinalBalance, User.Identity.GetUserName(), $"بانتظار تحصيل الرصيد النهائي المتبقي ({contract.RemainingBalance:N3} د.ل).");
                }
            }

            db.SaveChanges();
            TempData["Success"] = $"تم اعتماد محضر التسليم الرسمي #{docNo} بنجاح وإنجاز أمر التنفيذ!";
            return RedirectToAction("Details", "Requests", new { id = wo.KitchenRequestId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
