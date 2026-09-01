using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KOSS.Web.Models;

namespace KOSS.Web.Controllers
{
    [Authorize]
    public class FactoryController : Controller
    {
        private readonly KossDbContext db = new KossDbContext();

        // ──────────────────────────────────────────────
        //  GET: /Factory  -  لوحة تتبع التصنيع
        // ──────────────────────────────────────────────
        public ActionResult Index()
        {
            var contracts = db.Contracts
                .Include("Client")
                .Include("Units")
                .Where(c => c.Status == ContractStatus.UnderProduction ||
                            c.Status == ContractStatus.Manufactured    ||
                            c.Status == ContractStatus.Installed)
                .OrderByDescending(c => c.UpdatedAt)
                .ToList();
            return View(contracts);
        }

        // ──────────────────────────────────────────────
        //  POST: /Factory/MarkManufactured/5
        // ──────────────────────────────────────────────
        [HttpPost, Authorize(Roles = "FactoryManager,Executive")]
        public ActionResult MarkManufactured(int id, string notes)
        {
            var contract = db.Contracts.Include("Units").FirstOrDefault(c => c.Id == id);
            if (contract == null) return HttpNotFound();

            contract.Status    = ContractStatus.Manufactured;
            contract.UpdatedAt = DateTime.Now;
            contract.Notes     = notes;

            foreach (var unit in contract.Units)
                unit.ManufacturingStatus = UnitManufacturingStatus.Manufactured;

            LogAudit("Contracts", id, "StatusChange", "UnderProduction", "Manufactured",
                "تم إنهاء التصنيع في المصنع");

            db.SaveChanges();

            TempData["Success"] = "تم تحديث حالة العقد إلى (تم التصنيع). جاهز للتسليم والتركيب.";
            return RedirectToAction("Index");
        }

        // ──────────────────────────────────────────────
        //  POST: /Factory/MarkInstalled/5
        // ──────────────────────────────────────────────
        [HttpPost, Authorize(Roles = "FactoryManager,Executive")]
        public ActionResult MarkInstalled(int id, string technicianName, decimal laborMeters)
        {
            var contract = db.Contracts.Include("Units").FirstOrDefault(c => c.Id == id);
            if (contract == null) return HttpNotFound();

            contract.Status    = ContractStatus.Installed;
            contract.UpdatedAt = DateTime.Now;

            foreach (var unit in contract.Units)
            {
                unit.ManufacturingStatus = UnitManufacturingStatus.Installed;
                unit.DesignedBy          = technicianName; // إعادة استخدام الحقل للتسجيل
            }

            // حساب مكافأة الفني (مثلاً 50 د.ل / متر)
            decimal bonusRate     = 50m;
            decimal technicianBonus = laborMeters * bonusRate;

            // تسجيل مكافأة الفني
            var staff = db.StaffMembers.FirstOrDefault(s => s.FullName == technicianName);
            if (staff != null)
            {
                staff.BonusBalance += technicianBonus;
            }

            LogAudit("Contracts", id, "StatusChange", "Manufactured", "Installed",
                $"تم التركيب بواسطة {technicianName} - {laborMeters} متر");
            db.SaveChanges();

            TempData["Success"] = $"تم التركيب! مكافأة الفني: {technicianBonus:N3} د.ل ({laborMeters} متر × {bonusRate} د.ل/متر)";
            return RedirectToAction("Index");
        }

        // ──────────────────────────────────────────────
        //  GET: /Factory/Commission/5  -  إتمام التشطيب
        // ──────────────────────────────────────────────
        [Authorize(Roles = "Finance,Executive")]
        public ActionResult Commission(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var contract = db.Contracts
                .Include("Client")
                .Include("Payments")
                .FirstOrDefault(c => c.Id == id);
            if (contract == null) return HttpNotFound();

            ViewBag.FinalBalance = contract.TotalValue - contract.TotalPaid;
            return View(contract);
        }

        // ──────────────────────────────────────────────
        //  POST: /Factory/ConfirmCommission/5  -  قفل الملف
        // ──────────────────────────────────────────────
        [HttpPost, Authorize(Roles = "Finance,Executive"), ValidateAntiForgeryToken]
        public ActionResult ConfirmCommission(int contractId, decimal finalPayment)
        {
            var contract = db.Contracts
                .Include("Units")
                .FirstOrDefault(c => c.Id == contractId);
            if (contract == null) return HttpNotFound();

            // تسجيل الدفعة النهائية
            int lastId = db.Payments.Any() ? db.Payments.Max(p => p.Id) : 0;
            db.Payments.Add(new Payment
            {
                ContractId    = contractId,
                Amount        = finalPayment,
                PaymentType   = PaymentType.FinalPayment,
                ReceiptNumber = Helpers.PdfHelper.GenerateReceiptNumber(lastId),
                PaidAt        = DateTime.Now,
                ReceivedBy    = User.Identity.GetUserName(),
                Notes         = "الدفعة النهائية - تشطيب وتسليم"
            });

            contract.TotalPaid  += finalPayment;
            contract.Status      = ContractStatus.Completed;
            contract.UpdatedAt   = DateTime.Now;

            foreach (var unit in contract.Units)
                unit.ManufacturingStatus = UnitManufacturingStatus.Completed;

            LogAudit("Contracts", contractId, "StatusChange", "Installed", "Completed",
                "تم التشطيب والتسليم الرسمي للعميل");
            db.SaveChanges();

            TempData["Success"] = "🎉 تم إغلاق ملف العقد بنجاح. الدفعة النهائية مسجَّلة وكشف الحساب مقفول.";
            return RedirectToAction("Index");
        }

        // ──────────────────────────────────────────────
        //  مساعد تسجيل التدقيق
        // ──────────────────────────────────────────────
        private void LogAudit(string table, int recordId, string action,
            string oldVal, string newVal, string desc)
        {
            db.AuditLogs.Add(new AuditLog
            {
                TableName   = table,
                RecordId    = recordId,
                Action      = action,
                OldValue    = oldVal,
                NewValue    = newVal,
                Description = desc,
                ChangedBy   = User.Identity.GetUserName(),
                ChangedAt   = DateTime.Now
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
