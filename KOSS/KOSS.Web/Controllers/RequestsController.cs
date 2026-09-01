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
    public class RequestsController : Controller
    {
        private readonly KossDbContext db = new KossDbContext();

        // ──────────────────────────────────────────────
        //  GET: /Requests  -  قائمة طلبات المطابخ والمشاريع
        // ──────────────────────────────────────────────
        public ActionResult Index(string search, KitchenRequestStatus? status, ProjectType? projectType, int page = 1)
        {
            var query = db.KitchenRequests
                .Include(r => r.Customer)
                .Include(r => r.AssignedSalesStaff)
                .Include(r => r.Contracts)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.RequestNumber.Contains(search) ||
                                         r.Customer.Name.Contains(search) ||
                                         r.Customer.Phone.Contains(search) ||
                                         r.Location.Contains(search));
            }

            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }

            if (projectType.HasValue)
            {
                query = query.Where(r => r.ProjectType == projectType.Value);
            }

            int pageSize = 15;
            int total = query.Count();
            var requests = query
                .OrderByDescending(r => r.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.ProjectType = projectType;
            ViewBag.Page = page;
            ViewBag.Pages = (int)Math.Ceiling((double)total / pageSize);
            ViewBag.Total = total;

            return View(requests);
        }

        // ──────────────────────────────────────────────
        //  GET: /Requests/Create?customerId=5  -  إنشاء طلب مطبخ جديد
        // ──────────────────────────────────────────────
        public ActionResult Create(int? customerId)
        {
            if (customerId.HasValue)
            {
                var customer = db.Customers.Find(customerId.Value);
                if (customer != null)
                {
                    ViewBag.CustomerName = customer.Name;
                    ViewBag.CustomerId = customer.Id;
                }
            }

            ViewBag.Customers = new SelectList(db.Customers.OrderBy(c => c.Name).ToList(), "Id", "Name", customerId);
            ViewBag.Staff = new SelectList(db.StaffMembers.Where(s => s.IsActive).OrderBy(s => s.FullName).ToList(), "Id", "FullName");

            return View(new KitchenRequest
            {
                CustomerId = customerId ?? 0,
                TargetDeliveryDate = DateTime.Now.AddDays(30)
            });
        }

        // ──────────────────────────────────────────────
        //  POST: /Requests/Create
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(KitchenRequest request)
        {
            if (request.CustomerId <= 0)
            {
                ModelState.AddModelError("CustomerId", "يرجى اختيار العميل.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Customers = new SelectList(db.Customers.OrderBy(c => c.Name).ToList(), "Id", "Name", request.CustomerId);
                ViewBag.Staff = new SelectList(db.StaffMembers.Where(s => s.IsActive).OrderBy(s => s.FullName).ToList(), "Id", "FullName", request.AssignedSalesStaffId);
                return View(request);
            }

            // توليد رقم الطلب تلقائياً
            int lastId = db.KitchenRequests.Any() ? db.KitchenRequests.Max(r => r.Id) : 0;
            request.RequestNumber = $"REQ-{DateTime.Now.Year}-{(lastId + 1):D5}";
            request.Status = KitchenRequestStatus.RequestOpened;
            request.CreatedAt = DateTime.Now;
            request.UpdatedAt = DateTime.Now;
            request.CreatedBy = User.Identity.GetUserName();

            db.KitchenRequests.Add(request);
            db.SaveChanges();

            // تسجيل في السجل التاريخي
            RequestWorkflowEngine.Transition(db, request, KitchenRequestStatus.RequestOpened, User.Identity.GetUserName(), "إنشاء طلب مطبخ رسمي جديد للعميل.");
            db.SaveChanges();

            TempData["Success"] = $"تم فتح طلب المطبخ رقم {request.RequestNumber} بنجاح!";
            return RedirectToAction("Details", new { id = request.Id });
        }

        // ──────────────────────────────────────────────
        //  GET: /Requests/Details/5  -  لوحة المشروع المركزية الشاملة
        // ──────────────────────────────────────────────
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                var request = db.KitchenRequests
                    .Include("Customer")
                    .Include("AssignedSalesStaff")
                    .Include("StatusHistories")
                    .Include("SiteVisits.AssignedSurveyor")
                    .Include("DesignVersions.Designer")
                    .Include("Quotations.Items")
                    .Include("Contracts.PaymentSchedules")
                    .Include("Contracts.Payments")
                    .Include("WorkOrders.MaterialRequirements")
                    .Include("WorkOrders.Tasks")
                    .Include("WorkOrders.QualityChecks")
                    .Include("WorkOrders.HandoverDocuments")
                    .Include("Expenses")
                    .FirstOrDefault(r => r.Id == id.Value);

                if (request == null) return HttpNotFound();

                // حساب الربحية الراهنة
                ViewBag.Profitability = ProfitabilityCalculator.Calculate(request);
                ViewBag.ClosingCheck = RequestWorkflowEngine.VerifyClosingConditions(request);

                return View(request);
            }
            catch (Exception ex)
            {
                return Content("DETAILS_EXCEPTION: " + ex.ToString());
            }
        }

        // ──────────────────────────────────────────────
        //  POST: /Requests/Transition  -  تغيير حالة الطلب بمحرك الحالات
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Transition(int requestId, KitchenRequestStatus targetStatus, string reason)
        {
            var request = db.KitchenRequests
                .Include(r => r.Customer)
                .Include(r => r.SiteVisits)
                .Include(r => r.DesignVersions)
                .Include(r => r.Quotations)
                .Include(r => r.Contracts.Select(c => c.Payments))
                .Include(r => r.WorkOrders.Select(w => w.QualityChecks.Select(qc => qc.SnagItems)))
                .Include(r => r.WorkOrders.Select(w => w.HandoverDocuments))
                .FirstOrDefault(r => r.Id == requestId);

            if (request == null) return HttpNotFound();

            var check = RequestWorkflowEngine.CanTransition(request, targetStatus);
            if (!check.IsAllowed)
            {
                TempData["Error"] = check.ErrorMessage;
                return RedirectToAction("Details", new { id = requestId });
            }

            RequestWorkflowEngine.Transition(db, request, targetStatus, User.Identity.GetUserName(), string.IsNullOrEmpty(reason) ? "تحديث الحالة عبر النظام" : reason);
            db.SaveChanges();

            TempData["Success"] = $"تم تحديث حالة الطلب إلى [{targetStatus}] بنجاح!";
            return RedirectToAction("Details", new { id = requestId });
        }

        // ──────────────────────────────────────────────
        //  POST: /Requests/AddExpense  -  إضافة مصروف مباشر على مركز تكلفة المشروع
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddExpense(int kitchenRequestId, string expenseType, decimal amount, string paidTo = "", string receiptReference = "", string description = "")
        {
            if (amount <= 0)
            {
                TempData["Error"] = "يجب أن يكون مبلغ المصروف أكبر من صفر.";
                return RedirectToAction("Details", new { id = kitchenRequestId });
            }

            db.ProjectExpenses.Add(new ProjectExpense
            {
                KitchenRequestId = kitchenRequestId,
                ExpenseType = expenseType,
                Amount = amount,
                PaidTo = paidTo,
                ReceiptReference = receiptReference,
                Description = description,
                ExpenseDate = DateTime.Now,
                ApprovedBy = User.Identity.GetUserName()
            });

            db.SaveChanges();
            TempData["Success"] = $"تم تسجيل المصروف بقيمة {amount:N3} د.ل وإضافته لتكاليف المشروع.";
            return RedirectToAction("Details", new { id = kitchenRequestId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
