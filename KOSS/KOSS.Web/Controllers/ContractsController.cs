using System;
using System.Collections.Generic;
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
    public class ContractsController : Controller
    {
        private readonly KossDbContext db = new KossDbContext();

        // ──────────────────────────────────────────────
        //  GET: /Contracts  -  قائمة العقود الرسمية
        // ──────────────────────────────────────────────
        public ActionResult Index(ContractStatus? status)
        {
            var query = db.Contracts
                .Include(c => c.KitchenRequest)
                .Include(c => c.KitchenRequest.Customer)
                .Include(c => c.Quotation)
                .Include(c => c.PaymentSchedules)
                .Include(c => c.Payments)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(c => c.Status == status.Value);

            var list = query.OrderByDescending(c => c.CreatedAt).ToList();
            ViewBag.Status = status;
            return View(list);
        }

        // ──────────────────────────────────────────────
        //  GET: /Contracts/Create?requestId=5
        // ──────────────────────────────────────────────
        public ActionResult Create(int? requestId, int? clientId)
        {
            KitchenRequest request = null;
            if (requestId.HasValue)
            {
                request = db.KitchenRequests
                    .Include(r => r.Customer)
                    .Include(r => r.Quotations)
                    .Include(r => r.DesignVersions)
                    .FirstOrDefault(r => r.Id == requestId.Value);
            }
            else if (clientId.HasValue)
            {
                request = db.KitchenRequests.FirstOrDefault(r => r.CustomerId == clientId.Value);
            }

            if (request == null)
            {
                TempData["Error"] = "يرجى تحديد طلب مطبخ لإنشاء العقد.";
                return RedirectToAction("Index", "Requests");
            }

            var acceptedQuo = request.AcceptedQuotation ?? request.Quotations.OrderByDescending(q => q.Id).FirstOrDefault();
            var approvedDesign = request.ApprovedDesign ?? request.DesignVersions.OrderByDescending(d => d.Id).FirstOrDefault();

            decimal totalVal = acceptedQuo != null ? acceptedQuo.TotalAmount : 15000m;
            decimal deposit30 = totalVal * 0.30m;

            int lastId = db.Contracts.Any() ? db.Contracts.Max(c => c.Id) : 0;
            string contractNum = $"KOSS-CNT-{DateTime.Now.Year}-{(lastId + 1):D5}";

            ViewBag.Request = request;
            ViewBag.Quotation = acceptedQuo;
            ViewBag.Design = approvedDesign;

            return View(new Contract
            {
                KitchenRequestId = request.Id,
                ClientId = request.CustomerId,
                QuotationId = acceptedQuo?.Id,
                DesignVersionId = approvedDesign?.Id,
                ContractNumber = contractNum,
                TotalValue = totalVal,
                RequiredDeposit = deposit30,
                PricePerMeter = 850m,
                TotalMeters = approvedDesign?.EstimatedLinearMeters ?? 10m,
                SignedDate = DateTime.Now,
                TargetCompletionDate = DateTime.Now.AddDays(35)
            });
        }

        // ──────────────────────────────────────────────
        //  POST: /Contracts/Create
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(Contract contract)
        {
            if (!ModelState.IsValid)
            {
                return View(contract);
            }

            contract.Status = ContractStatus.AwaitingDeposit;
            contract.CreatedAt = DateTime.Now;
            contract.UpdatedAt = DateTime.Now;
            contract.CreatedBy = User.Identity.GetUserName();

            // إنشاء جدول الدفعات القياسي (30% عربون، 40% تصنيع، 20% تركيب، 10% تسليم)
            contract.PaymentSchedules = new List<PaymentSchedule>
            {
                new PaymentSchedule
                {
                    StageName = "عربون توقيع العقد (30%)",
                    Percentage = 30m,
                    Amount = contract.TotalValue * 0.30m,
                    DueDate = contract.SignedDate ?? DateTime.Now,
                    Condition = "عند توقيع العقد وقبل بدء التخطيط"
                },
                new PaymentSchedule
                {
                    StageName = "دفعة بدء التصنيع بالمصنع (40%)",
                    Percentage = 40m,
                    Amount = contract.TotalValue * 0.40m,
                    DueDate = (contract.SignedDate ?? DateTime.Now).AddDays(10),
                    Condition = "عند جاهزية المواد وبدء التصنيع"
                },
                new PaymentSchedule
                {
                    StageName = "دفعة الجاهزية للتركيب (20%)",
                    Percentage = 20m,
                    Amount = contract.TotalValue * 0.20m,
                    DueDate = (contract.SignedDate ?? DateTime.Now).AddDays(25),
                    Condition = "عند انتهاء التصنيع وفحص الجودة"
                },
                new PaymentSchedule
                {
                    StageName = "رصيد التسليم النهائي (10%)",
                    Percentage = 10m,
                    Amount = contract.TotalValue * 0.10m,
                    DueDate = contract.TargetCompletionDate ?? (contract.SignedDate ?? DateTime.Now).AddDays(35),
                    Condition = "عند توقيع محضر التسليم النهائي"
                }
            };

            db.Contracts.Add(contract);

            // تحديث حالة طلب المطبخ
            var request = db.KitchenRequests.Find(contract.KitchenRequestId);
            if (request != null)
            {
                RequestWorkflowEngine.Transition(db, request, KitchenRequestStatus.AwaitingContractAndDeposit, User.Identity.GetUserName(), $"تحرير العقد الرسمي {contract.ContractNumber} بقيمة {contract.TotalValue:N3} د.ل وبانتظار سداد العربون.");
            }

            db.SaveChanges();
            TempData["Success"] = $"تم إنشاء العقد رقم {contract.ContractNumber} بنجاح! تم إنشاء جدول الدفعات وبانتظار سداد العربون ({contract.RequiredDeposit:N3} د.ل).";
            return RedirectToAction("Details", "Requests", new { id = contract.KitchenRequestId });
        }

        // ──────────────────────────────────────────────
        //  POST: /Contracts/RecordDeposit  -  تسجيل سداد العربون وتفعيل العقد رسمياً
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RecordDeposit(int contractId, decimal amount, string paymentMethod, string notes)
        {
            var contract = db.Contracts.Include(c => c.KitchenRequest).Include(c => c.PaymentSchedules).FirstOrDefault(c => c.Id == contractId);
            if (contract == null) return HttpNotFound();

            if (amount <= 0)
            {
                TempData["Error"] = "يجب أن يكون مبلغ الدفعة أكبر من صفر.";
                return RedirectToAction("Details", "Requests", new { id = contract.KitchenRequestId });
            }

            // توليد رقم إيصال قبض
            int lastPayId = db.Payments.Any() ? db.Payments.Max(p => p.Id) : 0;
            string receiptNo = $"RCT-{DateTime.Now.Year}-{(lastPayId + 1):D5}";

            var payment = new Payment
            {
                ContractId = contract.Id,
                ReceiptNumber = receiptNo,
                Amount = amount,
                PaymentType = PaymentType.Deposit,
                PaymentMethod = paymentMethod == "تحويل مصرفي" ? PaymentMethod.BankTransfer : (paymentMethod == "صك مصدق" ? PaymentMethod.Cheque : PaymentMethod.Cash),
                ReceivedBy = User.Identity.GetUserName(),
                PaidAt = DateTime.Now,
                Notes = notes
            };

            db.Payments.Add(payment);
            contract.TotalPaid += amount;

            // تحديث جدول الدفعات
            var depositSched = contract.PaymentSchedules.FirstOrDefault(ps => ps.Percentage == 30m || ps.StageName.Contains("عربون"));
            if (depositSched != null)
            {
                depositSched.IsPaid = true;
                depositSched.PaidAt = DateTime.Now;
            }

            // إذا تم سداد العربون المطلوب، يتم تفعيل العقد وترقية حالة المشروع
            if (contract.TotalPaid >= contract.RequiredDeposit)
            {
                contract.Status = ContractStatus.Active;

                var request = contract.KitchenRequest;
                if (request != null)
                {
                    RequestWorkflowEngine.Transition(db, request, KitchenRequestStatus.ContractActive, User.Identity.GetUserName(), $"استلام دفعة العربون ({amount:N3} د.ل) بالإيصال #{receiptNo} وتفعيل العقد رسمياً.");
                    
                    // إنشاء أمر التنفيذ آلياً
                    int lastWoId = db.WorkOrders.Any() ? db.WorkOrders.Max(w => w.Id) : 0;
                    var wo = new WorkOrder
                    {
                        KitchenRequestId = request.Id,
                        ContractId = contract.Id,
                        OrderNumber = $"WO-{DateTime.Now.Year}-{(lastWoId + 1):D5}",
                        Priority = PriorityLevel.Normal,
                        Status = WorkOrderStatus.Planning,
                        PlannedStartDate = DateTime.Now,
                        ExpectedEndDate = contract.TargetCompletionDate,
                        CreatedAt = DateTime.Now,
                        CreatedBy = User.Identity.GetUserName()
                    };
                    db.WorkOrders.Add(wo);

                    RequestWorkflowEngine.Transition(db, request, KitchenRequestStatus.InPlanning, User.Identity.GetUserName(), $"إصدار أمر التنفيذ #{wo.OrderNumber} وبدء مرحلة التخطيط والخامات.");
                }
            }

            db.SaveChanges();
            TempData["Success"] = $"تم تسجيل إيصال القبض #{receiptNo} بمبلغ {amount:N3} د.ل وتفعيل العقد وأمر التنفيذ بنجاح!";
            return RedirectToAction("Details", "Requests", new { id = contract.KitchenRequestId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
