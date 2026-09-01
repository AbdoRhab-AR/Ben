using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KOSS.Web.Models;
using KOSS.Web.Helpers;

namespace KOSS.Web.Controllers
{
    [Authorize]
    public class SurveyController : Controller
    {
        private readonly KossDbContext db = new KossDbContext();

        // ──────────────────────────────────────────────
        //  GET: /Survey  -  قائمة العقود للمسح الميداني
        // ──────────────────────────────────────────────
        public ActionResult Index()
        {
            var contracts = db.Contracts
                .Include("Client")
                .Where(c => c.Status == ContractStatus.New ||
                            c.Status == ContractStatus.Measured)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
            return View(contracts);
        }

        // ──────────────────────────────────────────────
        //  GET: /Survey/EnterMeasurements/5
        // ──────────────────────────────────────────────
        [Authorize(Roles = "FieldSurveyor,Executive")]
        public ActionResult EnterMeasurements(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var contract = db.Contracts
                .Include("Client")
                .Include("Units")
                .FirstOrDefault(c => c.Id == id);
            if (contract == null) return HttpNotFound();
            return View(contract);
        }

        // ──────────────────────────────────────────────
        //  POST: /Survey/SaveMeasurements
        // ──────────────────────────────────────────────
        [HttpPost, Authorize(Roles = "FieldSurveyor,Executive"), ValidateAntiForgeryToken]
        public ActionResult SaveMeasurements(int contractId, KitchenUnit[] units)
        {
            var contract = db.Contracts.Include("Units").FirstOrDefault(c => c.Id == contractId);
            if (contract == null) return HttpNotFound();

            // حفظ القياسات لكل وحدة
            foreach (var unit in units ?? new KitchenUnit[0])
            {
                var existing = contract.Units.FirstOrDefault(u => u.Id == unit.Id);
                if (existing != null)
                {
                    existing.LengthCm  = unit.LengthCm;
                    existing.WidthCm   = unit.WidthCm;
                    existing.HeightCm  = unit.HeightCm;
                    existing.TotalArea = unit.TotalArea;
                }
            }

            // تحديث حالة العقد → تم القياس
            contract.Status    = ContractStatus.Measured;
            contract.UpdatedAt = DateTime.Now;
            db.SaveChanges();

            // تسجيل في سجل التدقيق
            db.AuditLogs.Add(new AuditLog
            {
                TableName   = "Contracts",
                RecordId    = contract.Id,
                Action      = "StatusChange",
                OldValue    = "New",
                NewValue    = "Measured",
                Description = "تم إدخال القياسات الميدانية",
                ChangedBy   = User.Identity.GetUserName(),
                ChangedAt   = DateTime.Now
            });
            db.SaveChanges();

            TempData["Success"] = "تم حفظ القياسات وتحديث حالة العقد إلى (تم القياس).";
            return RedirectToAction("PayFee", new { contractId = contract.Id });
        }

        // ──────────────────────────────────────────────
        //  GET: /Survey/PayFee/5  -  صفحة دفع رسوم التصميم
        // ──────────────────────────────────────────────
        public ActionResult PayFee(int? contractId)
        {
            if (contractId == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var contract = db.Contracts
                .Include("Client")
                .Include("Units")
                .Include("DesignFees")
                .FirstOrDefault(c => c.Id == contractId);
            if (contract == null) return HttpNotFound();

            // حساب الرسوم تلقائياً
            int unitCount = contract.Units.Count;
            ViewBag.CalculatedFee     = DesignFee.CalculateFee(unitCount);
            ViewBag.RequiresApproval  = DesignFee.RequiresManagerApproval(unitCount);
            ViewBag.UnitCount         = unitCount;

            return View(contract);
        }

        // ──────────────────────────────────────────────
        //  POST: /Survey/ConfirmPayFee
        // ──────────────────────────────────────────────
        [HttpPost, Authorize(Roles = "Finance,SalesStaff,Executive"), ValidateAntiForgeryToken]
        public ActionResult ConfirmPayFee(int contractId, decimal feeAmount, string notes)
        {
            var contract = db.Contracts
                .Include("Units")
                .Include("DesignFees")
                .FirstOrDefault(c => c.Id == contractId);
            if (contract == null) return HttpNotFound();

            // إنشاء سجل رسوم التصميم
            int lastId = db.Payments.Any() ? db.Payments.Max(p => p.Id) : 0;
            var fee = new DesignFee
            {
                ContractId    = contractId,
                UnitCount     = contract.Units.Count,
                FeeAmount     = feeAmount,
                IsPaid        = true,
                ReceiptNumber = PdfHelper.GenerateReceiptNumber(lastId),
                PaidAt        = DateTime.Now,
                ReceivedBy    = User.Identity.GetUserName(),
                Notes         = notes
            };
            db.DesignFees.Add(fee);

            // تحديث حالة العقد
            contract.Status    = ContractStatus.FeePaid;
            contract.UpdatedAt = DateTime.Now;
            db.SaveChanges();

            TempData["Success"]       = $"تم استلام رسوم التصميم بمبلغ {feeAmount:N3} د.ل. رقم الإيصال: {fee.ReceiptNumber}";
            TempData["ReceiptNumber"] = fee.ReceiptNumber;

            return RedirectToAction("Index", "Design");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
