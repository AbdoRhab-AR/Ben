using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KOSS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KOSS.Web.Controllers
{
    [Authorize]
    public class ContractsController : Controller
    {
        private readonly AppDbContext _context;

        public ContractsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(ContractStatus? status)
        {
            var query = _context.Contracts
                .Include(c => c.KitchenRequest)
                    .ThenInclude(r => r.Customer)
                .Include(c => c.PaymentSchedules)
                .Include(c => c.Payments)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(c => c.Status == status.Value);

            var list = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
            ViewBag.Status = status;
            return View(list);
        }

        public async Task<IActionResult> Create(int? requestId, int? quotationId)
        {
            KitchenRequest req = null;
            Quotation quo = null;
            DesignVersion des = null;

            if (requestId.HasValue)
            {
                req = await _context.KitchenRequests
                    .Include(r => r.Customer)
                    .Include(r => r.DesignVersions)
                    .Include(r => r.Quotations)
                    .FirstOrDefaultAsync(r => r.Id == requestId.Value);

                if (req != null)
                {
                    quo = quotationId.HasValue ? req.Quotations.FirstOrDefault(q => q.Id == quotationId.Value)
                                               : req.Quotations.FirstOrDefault(q => q.Status == QuotationStatus.Accepted) ?? req.Quotations.LastOrDefault();

                    des = req.DesignVersions.FirstOrDefault(d => d.Status == DesignVersionStatus.ApprovedByCustomer) ?? req.DesignVersions.LastOrDefault();
                }
            }

            ViewBag.Request = req;
            ViewBag.Quotation = quo;
            ViewBag.Design = des;

            decimal totalVal = quo != null ? quo.TotalAmount : 12500m;
            decimal deposit = totalVal * 0.30m;

            var model = new Contract
            {
                KitchenRequestId = requestId ?? (req?.Id ?? 0),
                QuotationId = quo?.Id,
                DesignVersionId = des?.Id,
                ContractNumber = $"CNT-{DateTime.Now.Year}-{new Random().Next(1000, 9999)}",
                SignedDate = DateTime.Now,
                TargetCompletionDate = DateTime.Now.AddDays(35),
                TotalValue = totalVal,
                RequiredDeposit = deposit,
                PricePerMeter = des != null && des.EstimatedLinearMeters > 0 ? (totalVal / des.EstimatedLinearMeters.Value) : 950,
                TotalMeters = des?.EstimatedLinearMeters ?? 6,
                Status = ContractStatus.Draft
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.Now;
                model.UpdatedAt = DateTime.Now;
                model.CreatedBy = User.Identity?.Name ?? "Admin";
                model.Status = ContractStatus.AwaitingDeposit;

                if (model.RequiredDeposit <= 0)
                {
                    model.RequiredDeposit = model.TotalValue * 0.30m;
                }

                model.PaymentSchedules = new List<PaymentSchedule>
                {
                    new PaymentSchedule { StageName = "عربون التعاقد", Percentage = 30, Amount = model.TotalValue * 0.30m, DueDate = DateTime.Now, Condition = "عند توقيع العقد", IsPaid = false },
                    new PaymentSchedule { StageName = "دفعة بدء التصنيع والقص", Percentage = 40, Amount = model.TotalValue * 0.40m, DueDate = DateTime.Now.AddDays(10), Condition = "عند تجهيز وقص الخامات بالمصنع", IsPaid = false },
                    new PaymentSchedule { StageName = "دفعة بدء التركيب والتوريد", Percentage = 20, Amount = model.TotalValue * 0.20m, DueDate = DateTime.Now.AddDays(25), Condition = "عند توريد المطبخ للموقع", IsPaid = false },
                    new PaymentSchedule { StageName = "مخالصة التسليم النهائي", Percentage = 10, Amount = model.TotalValue * 0.10m, DueDate = model.TargetCompletionDate ?? DateTime.Now.AddDays(35), Condition = "عند توقيع محضر التسليم", IsPaid = false }
                };

                _context.Contracts.Add(model);

                var req = await _context.KitchenRequests.FindAsync(model.KitchenRequestId);
                if (req != null)
                {
                    req.Status = KitchenRequestStatus.AwaitingContractAndDeposit;
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = $"تم حفظ العقد الرسمي ({model.ContractNumber}) وتوليد جدول الدفعات الرباعي بنجاح.";
                return RedirectToAction("Details", "Requests", new { id = model.KitchenRequestId });
            }

            ViewBag.Request = await _context.KitchenRequests.Include(r => r.Customer).FirstOrDefaultAsync(r => r.Id == model.KitchenRequestId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordDeposit(int contractId, decimal amount, PaymentMethod method, string referenceNumber)
        {
            var contract = await _context.Contracts
                .Include(c => c.KitchenRequest)
                .Include(c => c.PaymentSchedules)
                .FirstOrDefaultAsync(c => c.Id == contractId);

            if (contract == null) return NotFound();

            if (amount <= 0)
            {
                TempData["Error"] = "يرجى إدخال مبلغ صحيح لإيصال القبض.";
                return RedirectToAction("Details", "Requests", new { id = contract.KitchenRequestId });
            }

            var payment = new Payment
            {
                ContractId = contractId,
                ReceiptNumber = $"REC-{DateTime.Now.Year}-{new Random().Next(1000, 9999)}",
                Amount = amount,
                PaymentType = PaymentType.Deposit,
                PaymentMethod = method,
                ReferenceNumber = referenceNumber,
                PaidAt = DateTime.Now,
                ReceivedBy = User.Identity?.Name ?? "Admin",
                Notes = "سداد عربون تفعيل العقد والبدء في أمر التشغيل"
            };

            _context.Payments.Add(payment);
            contract.TotalPaid += amount;

            var depositStage = contract.PaymentSchedules.FirstOrDefault(s => s.Percentage == 30);
            if (depositStage != null)
            {
                depositStage.IsPaid = true;
                depositStage.PaidAt = DateTime.Now;
            }

            if (contract.TotalPaid >= contract.RequiredDeposit)
            {
                contract.Status = ContractStatus.Active;
                var req = contract.KitchenRequest;
                if (req != null)
                {
                    req.Status = KitchenRequestStatus.ContractActive;

                    var existingWo = await _context.WorkOrders.FirstOrDefaultAsync(w => w.KitchenRequestId == req.Id);
                    if (existingWo == null)
                    {
                        var wo = new WorkOrder
                        {
                            KitchenRequestId = req.Id,
                            ContractId = contract.Id,
                            OrderNumber = $"WO-{DateTime.Now.Year}-{new Random().Next(1000, 9999)}",
                            ExpectedEndDate = contract.TargetCompletionDate ?? DateTime.Now.AddDays(25),
                            Status = WorkOrderStatus.Planning,
                            CreatedBy = User.Identity?.Name ?? "Admin",
                            MaterialRequirements = new List<MaterialRequirement>
                            {
                                new MaterialRequirement { ItemName = "ألواح خشب HDF إسباني 18 ملم", Category = "أخشاب", QuantityRequired = 14, Unit = "لوح", EstimatedUnitCost = 185 },
                                new MaterialRequirement { ItemName = "ألواح خشب أبيض ميلامين داخلي", Category = "أخشاب", QuantityRequired = 10, Unit = "لوح", EstimatedUnitCost = 120 },
                                new MaterialRequirement { ItemName = "شريط حواف PVC تركي 2 ملم", Category = "إكسسوارات", QuantityRequired = 80, Unit = "متر", EstimatedUnitCost = 2.5m },
                                new MaterialRequirement { ItemName = "مفصلات هيدروليك ناعمة الإغلاق Blum", Category = "إكسسوارات", QuantityRequired = 28, Unit = "قطعة", EstimatedUnitCost = 14 },
                                new MaterialRequirement { ItemName = "سحابات أدراج مخفية Tandembox", Category = "إكسسوارات", QuantityRequired = 6, Unit = "طقم", EstimatedUnitCost = 85 },
                                new MaterialRequirement { ItemName = "مقابض ألمنيوم مخفية Gola Profile", Category = "ألمنيوم", QuantityRequired = 12, Unit = "متر", EstimatedUnitCost = 35 }
                            },
                            Tasks = new List<ManufacturingTask>
                            {
                                new ManufacturingTask { TaskName = "1. قص الألواح وتجهيز الهياكل", Status = "Pending" },
                                new ManufacturingTask { TaskName = "2. لصق شريط الحواف CNC", Status = "Pending" },
                                new ManufacturingTask { TaskName = "3. التثقيب والتجميع الميكانيكي", Status = "Pending" },
                                new ManufacturingTask { TaskName = "4. فحص الجودة والتغليف", Status = "Pending" }
                            }
                        };
                        _context.WorkOrders.Add(wo);
                    }

                    _context.RequestStatusHistories.Add(new RequestStatusHistory
                    {
                        KitchenRequestId = req.Id,
                        OldStatus = KitchenRequestStatus.AwaitingContractAndDeposit,
                        NewStatus = KitchenRequestStatus.ContractActive,
                        ChangedBy = User.Identity?.Name ?? "Admin",
                        Notes = $"تم قبض العربون ({amount:N3} د.ل) وتفعيل العقد رسمياً وإصدار أمر التنفيذ والـ BOM."
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"تم تسجيل إيصال القبض ({payment.ReceiptNumber}) بقيمة {amount:N3} د.ل وتفعيل العقد بنجاح.";
            return RedirectToAction("Details", "Requests", new { id = contract.KitchenRequestId });
        }
    }
}
