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
    public class QuotationsController : Controller
    {
        private readonly AppDbContext _context;

        public QuotationsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(QuotationStatus? status)
        {
            var query = _context.Quotations
                .Include(q => q.KitchenRequest)
                    .ThenInclude(r => r.Customer)
                .Include(q => q.DesignVersion)
                .Include(q => q.Items)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(q => q.Status == status.Value);

            var list = await query.OrderByDescending(q => q.CreatedAt).ToListAsync();
            ViewBag.Status = status;
            return View(list);
        }

        public async Task<IActionResult> Create(int requestId)
        {
            var req = await _context.KitchenRequests
                .Include(r => r.Customer)
                .Include(r => r.DesignVersions)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (req == null) return NotFound();

            var approvedDesign = req.DesignVersions.FirstOrDefault(d => d.Status == DesignVersionStatus.ApprovedByCustomer)
                                 ?? req.DesignVersions.LastOrDefault();

            ViewBag.Request = req;
            ViewBag.Design = approvedDesign;

            var model = new Quotation
            {
                KitchenRequestId = requestId,
                DesignVersionId = approvedDesign?.Id,
                QuotationNumber = $"Q-{DateTime.Now.Year}-{new Random().Next(1000, 9999)}",
                ValidityDays = 15,
                Items = new List<QuotationItem>
                {
                    new QuotationItem { ItemName = "خزائن مطبخ سفلية وعلوية كاملة (خشب إسباني مقاوم)", Category = QuotationItemCategory.WoodMaterials, Quantity = approvedDesign?.EstimatedLinearMeters ?? 6, Unit = "متر طولي", UnitPrice = 850 },
                    new QuotationItem { ItemName = "سطح رخام كوارتز طبيعي مع القص والتشطيب", Category = QuotationItemCategory.Countertops, Quantity = approvedDesign?.EstimatedLinearMeters ?? 6, Unit = "متر طولي", UnitPrice = 320 },
                    new QuotationItem { ItemName = "إكسسوارات ومفصلات Blum هيدروليك نمساوي", Category = QuotationItemCategory.HardwareAndAccessories, Quantity = 1, Unit = "مجموعة", UnitPrice = 950 },
                    new QuotationItem { ItemName = "أعمال التوريد والتركيب الميداني وضمان الجودة", Category = QuotationItemCategory.InstallationAndDelivery, Quantity = 1, Unit = "خدمة", UnitPrice = 600 }
                }
            };

            model.SubTotal = model.Items.Sum(i => i.TotalPrice);
            model.TotalAmount = model.SubTotal;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Quotation model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.Now;
                model.CreatedBy = User.Identity?.Name ?? "Admin";
                model.Status = QuotationStatus.Draft;

                if (model.Items != null && model.Items.Any())
                {
                    foreach (var itm in model.Items)
                    {
                        itm.TotalPrice = Math.Max(0, (itm.Quantity * itm.UnitPrice) - itm.Discount);
                    }
                    model.SubTotal = model.Items.Sum(i => i.TotalPrice);
                }

                model.TotalAmount = Math.Max(0, model.SubTotal - model.Discount + model.TaxAmount);

                _context.Quotations.Add(model);

                var req = await _context.KitchenRequests.FindAsync(model.KitchenRequestId);
                if (req != null)
                {
                    req.Status = KitchenRequestStatus.QuotationSent;
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = $"تم حفظ عرض السعر رقم ({model.QuotationNumber}) بنجاح.";
                return RedirectToAction("Details", "Requests", new { id = model.KitchenRequestId });
            }

            ViewBag.Request = await _context.KitchenRequests.Include(r => r.Customer).FirstOrDefaultAsync(r => r.Id == model.KitchenRequestId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int quotationId)
        {
            var quo = await _context.Quotations.Include(q => q.KitchenRequest).FirstOrDefaultAsync(q => q.Id == quotationId);
            if (quo == null) return NotFound();

            quo.Status = QuotationStatus.Accepted;
            var req = quo.KitchenRequest;
            if (req != null)
            {
                req.Status = KitchenRequestStatus.AwaitingContractAndDeposit;
                _context.RequestStatusHistories.Add(new RequestStatusHistory
                {
                    KitchenRequestId = req.Id,
                    OldStatus = KitchenRequestStatus.QuotationSent,
                    NewStatus = KitchenRequestStatus.AwaitingContractAndDeposit,
                    ChangedBy = User.Identity?.Name ?? "Admin",
                    Notes = $"وافق العميل رسمياً على عرض السعر {quo.QuotationNumber} بقيمة {quo.TotalAmount:N3} د.ل. تم التحويل لتحرير العقد وسداد العربون."
                });
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "تم قبول عرض السعر بنجاح، والمشروع جاهز لتحرير العقد الرسمي وسداد العربون.";
            return RedirectToAction("Create", "Contracts", new { requestId = quo.KitchenRequestId, quotationId = quo.Id });
        }
    }
}
