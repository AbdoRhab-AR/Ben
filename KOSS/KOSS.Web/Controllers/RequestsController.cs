using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KOSS.Web.Models;
using KOSS.Web.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KOSS.Web.Controllers
{
    [Authorize]
    public class RequestsController : Controller
    {
        private readonly AppDbContext _context;

        public RequestsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(KitchenRequestStatus? status, string search)
        {
            var query = _context.KitchenRequests
                .Include(r => r.Customer)
                .Include(r => r.Contracts)
                .Include(r => r.WorkOrders)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(r => r.Status == status.Value);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(r => r.RequestNumber.Contains(search) || r.Customer.Name.Contains(search) || r.Customer.Phone.Contains(search) || r.Location.Contains(search));

            var list = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
            ViewBag.Status = status;
            ViewBag.Search = search;
            return View(list);
        }

        public async Task<IActionResult> Create(int? customerId)
        {
            ViewBag.Customers = await _context.Customers.OrderBy(c => c.Name).ToListAsync();
            var model = new KitchenRequest
            {
                RequestNumber = $"REQ-{DateTime.Now.Year}-{new Random().Next(1000, 9999)}",
                CustomerId = customerId ?? 0,
                Status = KitchenRequestStatus.RequestOpened
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KitchenRequest model)
        {
            if (model.CustomerId == 0)
            {
                ModelState.AddModelError("CustomerId", "يرجى اختيار العميل أو تسجيل عميل جديد أولاً.");
            }

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.RequestNumber))
                {
                    model.RequestNumber = $"REQ-{DateTime.Now.Year}-{new Random().Next(10000, 99999)}";
                }

                model.CreatedAt = DateTime.Now;
                model.UpdatedAt = DateTime.Now;
                model.CreatedBy = User.Identity?.Name ?? "Admin";
                model.Status = KitchenRequestStatus.AwaitingSiteVisit;

                _context.KitchenRequests.Add(model);
                await _context.SaveChangesAsync();

                _context.RequestStatusHistories.Add(new RequestStatusHistory
                {
                    KitchenRequestId = model.Id,
                    OldStatus = KitchenRequestStatus.RequestOpened,
                    NewStatus = KitchenRequestStatus.AwaitingSiteVisit,
                    ChangedBy = User.Identity?.Name ?? "Admin",
                    Notes = "تم فتح الطلب وتحويله تلقائياً لانتظار المعاينة والقياسات الميدانية."
                });
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = model.Id });
            }

            ViewBag.Customers = await _context.Customers.OrderBy(c => c.Name).ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var req = await _context.KitchenRequests
                .Include(r => r.Customer)
                .Include(r => r.SiteVisits)
                    .ThenInclude(v => v.AssignedSurveyor)
                .Include(r => r.DesignVersions)
                    .ThenInclude(d => d.Designer)
                .Include(r => r.Quotations)
                    .ThenInclude(q => q.Items)
                .Include(r => r.Contracts)
                    .ThenInclude(c => c.PaymentSchedules)
                .Include(r => r.Contracts)
                    .ThenInclude(c => c.Payments)
                .Include(r => r.WorkOrders)
                    .ThenInclude(w => w.MaterialRequirements)
                .Include(r => r.WorkOrders)
                    .ThenInclude(w => w.Tasks)
                .Include(r => r.WorkOrders)
                    .ThenInclude(w => w.QualityChecks)
                .Include(r => r.WorkOrders)
                    .ThenInclude(w => w.InstallationOrders)
                .Include(r => r.WorkOrders)
                    .ThenInclude(w => w.HandoverDocuments)
                .Include(r => r.Expenses)
                .Include(r => r.StatusHistories)
                .Include(r => r.CabinetUnits)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (req == null) return NotFound();

            var profitability = ProfitabilityCalculator.Calculate(req);
            ViewBag.Profitability = profitability;
            ViewBag.ClosingCheck = RequestWorkflowEngine.VerifyClosingConditions(req);
            ViewBag.PricingSummary = LibyanPricingEngine.GeneratePricingSummary(req);

            return View(req);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransitionStatus(int requestId, KitchenRequestStatus targetStatus, string reason)
        {
            var req = await _context.KitchenRequests
                .Include(r => r.Contracts)
                .Include(r => r.WorkOrders)
                    .ThenInclude(w => w.QualityChecks)
                .Include(r => r.WorkOrders)
                    .ThenInclude(w => w.HandoverDocuments)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (req == null) return NotFound();

            var check = RequestWorkflowEngine.CanTransition(req, targetStatus);
            if (!check.IsAllowed)
            {
                TempData["Error"] = check.ErrorMessage;
                return RedirectToAction("Details", new { id = requestId });
            }

            var oldStatus = req.Status;
            req.Status = targetStatus;
            req.UpdatedAt = DateTime.Now;

            _context.RequestStatusHistories.Add(new RequestStatusHistory
            {
                KitchenRequestId = req.Id,
                OldStatus = oldStatus,
                NewStatus = targetStatus,
                ChangedBy = User.Identity?.Name ?? "Admin",
                Notes = reason ?? $"تغيير الحالة يدوياً من {oldStatus} إلى {targetStatus}"
            });

            await _context.SaveChangesAsync();
            TempData["Success"] = $"تم تحديث حالة المشروع بنجاح إلى: {targetStatus}";
            return RedirectToAction("Details", new { id = requestId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExpense(int kitchenRequestId, string expenseType, decimal amount, string invoiceNumber, string notes)
        {
            if (amount <= 0)
            {
                TempData["Error"] = "يجب أن تكون قيمة المصروف أكبر من الصفر.";
                return RedirectToAction("Details", new { id = kitchenRequestId });
            }

            var expense = new ProjectExpense
            {
                KitchenRequestId = kitchenRequestId,
                ExpenseType = expenseType,
                Amount = amount,
                ReceiptReference = invoiceNumber,
                Description = notes,
                ApprovedBy = User.Identity?.Name ?? "Admin",
                ExpenseDate = DateTime.Now
            };

            _context.ProjectExpenses.Add(expense);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"تم قيد المصروف ({amount:N3} د.ل) بنجاح على مركز تكلفة المشروع.";
            return RedirectToAction("Details", new { id = kitchenRequestId });
        }
    }
}
