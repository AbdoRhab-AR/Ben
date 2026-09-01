using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KOSS.Web.Models;

namespace KOSS.Web.Controllers
{
    [Authorize]
    public class FinanceController : Controller
    {
        private readonly KossDbContext db = new KossDbContext();

        // ──────────────────────────────────────────────
        //  GET: /Finance  -  لوحة المتابعة المالية
        // ──────────────────────────────────────────────
        [Authorize(Roles = "Finance,Executive")]
        public ActionResult Index()
        {
            var contracts = db.Contracts
                .Include("Client")
                .Include("Payments")
                .Include("Units")
                .OrderByDescending(c => c.UpdatedAt)
                .ToList();

            // سعر المتر الحالي
            ViewBag.CurrentPricePerMeter = db.Contracts
                .OrderByDescending(c => c.UpdatedAt)
                .Select(c => c.PricePerMeter)
                .FirstOrDefault();

            return View(contracts);
        }

        // ──────────────────────────────────────────────
        //  GET: /Finance/RecordPayment/5
        // ──────────────────────────────────────────────
        [Authorize(Roles = "Finance,Executive")]
        public ActionResult RecordPayment(int? contractId)
        {
            if (contractId == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var contract = db.Contracts
                .Include("Client")
                .Include("Units")
                .FirstOrDefault(c => c.Id == contractId);
            if (contract == null) return HttpNotFound();

            var vm = new Models.ViewModels.RecordPaymentViewModel
            {
                ContractId      = contract.Id,
                ClientName      = contract.Client?.Name,
                ContractNumber  = contract.ContractNumber,
                TotalValue      = contract.TotalValue,
                TotalPaid       = contract.TotalPaid
            };
            return View(vm);
        }

        // ──────────────────────────────────────────────
        //  POST: /Finance/RecordPayment
        // ──────────────────────────────────────────────
        [HttpPost, Authorize(Roles = "Finance,Executive"), ValidateAntiForgeryToken]
        public ActionResult RecordPayment(Models.ViewModels.RecordPaymentViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var contract = db.Contracts
                .Include("Units")
                .FirstOrDefault(c => c.Id == vm.ContractId);
            if (contract == null) return HttpNotFound();

            // إنشاء رقم إيصال فريد
            int lastId = db.Payments.Any() ? db.Payments.Max(p => p.Id) : 0;
            string receiptNo = Helpers.PdfHelper.GenerateReceiptNumber(lastId);

            // حفظ الدفعة
            var payment = new Payment
            {
                ContractId     = vm.ContractId,
                ReceiptNumber  = receiptNo,
                Amount         = vm.Amount,
                PaymentType    = vm.PaymentType,
                PaymentMethod  = vm.PaymentMethod,
                ReferenceNumber = vm.ReferenceNumber,
                Notes          = vm.Notes,
                PaidAt         = DateTime.Now,
                ReceivedBy     = User.Identity.GetUserName()
            };
            db.Payments.Add(payment);

            // تحديث المدفوع في العقد
            contract.TotalPaid += vm.Amount;
            contract.UpdatedAt  = DateTime.Now;

            // تشغيل محرك توزيع العربون 70%
            var units = contract.Units.OrderBy(u => u.Priority).ToList();
            var allocationResults = Helpers.DepositAllocator.Allocate(units, contract.TotalPaid);
            vm.AllocationResults = allocationResults;

            // تحديث حالة العقد بناء على نتيجة التخصيص
            bool anyActive = units.Any(u => u.ManufacturingStatus == UnitManufacturingStatus.Active);
            if (anyActive && contract.Status == ContractStatus.Designed)
                contract.Status = ContractStatus.Active;

            db.SaveChanges();

            // تسجيل في سجل التدقيق
            db.AuditLogs.Add(new AuditLog
            {
                TableName   = "Payments",
                RecordId    = payment.Id,
                Action      = "Create",
                NewValue    = $"{vm.Amount:N3} د.ل",
                Description = $"دفعة مالية - إيصال {receiptNo}",
                ChangedBy   = User.Identity.GetUserName(),
                ChangedAt   = DateTime.Now
            });
            db.SaveChanges();

            TempData["Success"]  = $"تم تسجيل الدفعة بنجاح. رقم الإيصال: {receiptNo}";
            ViewBag.ReceiptNo    = receiptNo;
            ViewBag.ShowAllocation = true;

            return View(vm);
        }

        // ──────────────────────────────────────────────
        //  GET: /Finance/DownloadReceipt/5  -  تنزيل PDF
        // ──────────────────────────────────────────────
        [Authorize(Roles = "Finance,Executive")]
        public ActionResult DownloadReceipt(int id)
        {
            var payment = db.Payments
                .Include("Contract")
                .Include("Contract.Client")
                .FirstOrDefault(p => p.Id == id);
            if (payment == null) return HttpNotFound();

            byte[] pdfBytes = Helpers.PdfHelper.GeneratePaymentReceipt(
                payment, payment.Contract, payment.Contract.Client);

            return File(pdfBytes, "application/pdf", $"إيصال-{payment.ReceiptNumber}.pdf");
        }

        // ──────────────────────────────────────────────
        //  GET: /Finance/UpdatePrice  -  تحديث سعر المتر
        // ──────────────────────────────────────────────
        [Authorize(Roles = "Finance,Executive")]
        public ActionResult UpdatePrice()
        {
            decimal current = 0;
            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Application["CurrentPricePerMeter"] != null)
            {
                current = (decimal)System.Web.HttpContext.Current.Application["CurrentPricePerMeter"];
            }
            else if (db.Contracts.Any())
            {
                current = db.Contracts
                    .OrderByDescending(c => c.UpdatedAt)
                    .Select(c => c.PricePerMeter)
                    .FirstOrDefault();
            }
            else
            {
                current = 850m; // سعر افتراضي
            }

            var vm = new Models.ViewModels.UpdatePriceViewModel { CurrentPrice = current, NewPricePerMeter = current };
            return View(vm);
        }

        // ──────────────────────────────────────────────
        //  POST: /Finance/UpdatePrice
        // ──────────────────────────────────────────────
        [HttpPost, Authorize(Roles = "Finance,Executive"), ValidateAntiForgeryToken]
        public ActionResult UpdatePrice(Models.ViewModels.UpdatePriceViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // تخزين السعر الجديد في التطبيق (AppSettings أو جدول خاص)
            System.Web.HttpContext.Current.Application["CurrentPricePerMeter"] = vm.NewPricePerMeter;

            // تسجيل في سجل التدقيق
            db.AuditLogs.Add(new AuditLog
            {
                TableName   = "System",
                RecordId    = 0,
                Action      = "PriceUpdate",
                OldValue    = vm.CurrentPrice.ToString("N3"),
                NewValue    = vm.NewPricePerMeter.ToString("N3"),
                Description = $"تحديث سعر المتر: {vm.Reason}",
                ChangedBy   = User.Identity.GetUserName(),
                ChangedAt   = DateTime.Now
            });
            db.SaveChanges();

            TempData["PriceAlert"]   = $"تم تحديث سعر المتر إلى {vm.NewPricePerMeter:N3} د.ل بتاريخ {DateTime.Now:yyyy/MM/dd HH:mm}";
            TempData["PriceUpdated"] = true;

            return RedirectToAction("Index");
        }

        // ──────────────────────────────────────────────
        //  GET: /Finance/Ledger/5  -  كشف حساب عقد
        // ──────────────────────────────────────────────
        [Authorize(Roles = "Finance,Executive")]
        public ActionResult Ledger(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            var contract = db.Contracts
                .Include("Client")
                .Include("Payments")
                .Include("Units")
                .Include("DesignFees")
                .FirstOrDefault(c => c.Id == id);
            if (contract == null) return HttpNotFound();
            return View(contract);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
