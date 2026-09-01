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
    public class QuotationsController : Controller
    {
        private readonly KossDbContext db = new KossDbContext();

        // ──────────────────────────────────────────────
        //  GET: /Quotations  -  قائمة عروض الأسعار
        // ──────────────────────────────────────────────
        public ActionResult Index(QuotationStatus? status)
        {
            var query = db.Quotations
                .Include(q => q.KitchenRequest)
                .Include(q => q.KitchenRequest.Customer)
                .Include(q => q.DesignVersion)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(q => q.Status == status.Value);

            var list = query.OrderByDescending(q => q.CreatedAt).ToList();
            ViewBag.Status = status;
            return View(list);
        }

        // ──────────────────────────────────────────────
        //  GET: /Quotations/Create?requestId=5
        // ──────────────────────────────────────────────
        public ActionResult Create(int? requestId)
        {
            if (requestId == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var request = db.KitchenRequests
                .Include(r => r.Customer)
                .Include(r => r.DesignVersions)
                .FirstOrDefault(r => r.Id == requestId.Value);

            if (request == null) return HttpNotFound();

            var approvedDesign = request.ApprovedDesign ?? request.DesignVersions.OrderByDescending(d => d.Id).FirstOrDefault();
            if (approvedDesign == null)
            {
                TempData["Error"] = "لا يمكن إعداد عرض سعر دون وجود إصدار تصميم معتمد للطلب.";
                return RedirectToAction("Details", "Requests", new { id = requestId });
            }

            int lastId = db.Quotations.Any() ? db.Quotations.Max(q => q.Id) : 0;
            string quoNum = $"QUO-{DateTime.Now.Year}-{(lastId + 1):D5}";

            ViewBag.Request = request;
            ViewBag.Design = approvedDesign;

            return View(new Quotation
            {
                KitchenRequestId = request.Id,
                DesignVersionId = approvedDesign.Id,
                QuotationNumber = quoNum,
                ValidityDays = 15,
                TotalAmount = (approvedDesign.EstimatedLinearMeters ?? 5) * 850m
            });
        }

        // ──────────────────────────────────────────────
        //  POST: /Quotations/Create
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(Quotation quotation, List<QuotationItem> items)
        {
            if (quotation.TotalAmount <= 0 && items != null && items.Any())
            {
                quotation.SubTotal = items.Sum(i => i.Quantity * i.UnitPrice);
                quotation.TotalAmount = quotation.SubTotal - quotation.Discount + quotation.TaxAmount;
            }

            quotation.Status = QuotationStatus.Draft;
            quotation.CreatedAt = DateTime.Now;
            quotation.CreatedBy = User.Identity.GetUserName();

            if (items != null)
            {
                foreach (var item in items)
                {
                    if (!string.IsNullOrEmpty(item.ItemName))
                    {
                        item.TotalPrice = (item.Quantity * item.UnitPrice) - item.Discount;
                        quotation.Items.Add(item);
                    }
                }
            }

            // بند افتراضي إذا لم يتم إدخال بنود مفصلة
            if (!quotation.Items.Any())
            {
                quotation.Items.Add(new QuotationItem
                {
                    Category = QuotationItemCategory.WoodMaterials,
                    ItemName = "مطبخ متكامل حسب التصميم والمواصفات المعتمدة",
                    Unit = "متر",
                    Quantity = 1,
                    UnitPrice = quotation.TotalAmount,
                    TotalPrice = quotation.TotalAmount
                });
            }

            db.Quotations.Add(quotation);
            db.SaveChanges();

            // تحديث حالة الطلب إلى قيد التسعير
            var request = db.KitchenRequests.Find(quotation.KitchenRequestId);
            if (request != null && request.Status <= KitchenRequestStatus.InPricing)
            {
                RequestWorkflowEngine.Transition(db, request, KitchenRequestStatus.InPricing, User.Identity.GetUserName(), $"إعداد عرض السعر {quotation.QuotationNumber}");
            }

            db.SaveChanges();
            TempData["Success"] = $"تم حفظ مسودة عرض السعر {quotation.QuotationNumber} بنجاح!";
            return RedirectToAction("Details", "Requests", new { id = quotation.KitchenRequestId });
        }

        // ──────────────────────────────────────────────
        //  POST: /Quotations/SendToCustomer
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SendToCustomer(int quotationId)
        {
            var quo = db.Quotations.Include(q => q.KitchenRequest).FirstOrDefault(q => q.Id == quotationId);
            if (quo == null) return HttpNotFound();

            quo.Status = QuotationStatus.SentToCustomer;
            quo.SentToCustomerAt = DateTime.Now;

            var request = quo.KitchenRequest;
            if (request != null)
            {
                RequestWorkflowEngine.Transition(db, request, KitchenRequestStatus.QuotationSent, User.Identity.GetUserName(), $"إرسال عرض السعر {quo.QuotationNumber} للعميل بقيمة {quo.TotalAmount:N3} د.ل.");
            }

            db.SaveChanges();
            TempData["Success"] = $"تم إرسال عرض السعر {quo.QuotationNumber} للعميل بنجاح.";
            return RedirectToAction("Details", "Requests", new { id = quo.KitchenRequestId });
        }

        // ──────────────────────────────────────────────
        //  POST: /Quotations/Accept  -  قبول العميل للعرض والانتقال لمرحلة التعاقد
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Accept(int quotationId)
        {
            var quo = db.Quotations.Include(q => q.KitchenRequest).FirstOrDefault(q => q.Id == quotationId);
            if (quo == null) return HttpNotFound();

            quo.Status = QuotationStatus.Accepted;
            quo.AcceptedAt = DateTime.Now;

            var request = quo.KitchenRequest;
            if (request != null)
            {
                RequestWorkflowEngine.Transition(db, request, KitchenRequestStatus.QuotationAccepted, User.Identity.GetUserName(), $"قبول العميل لعرض السعر {quo.QuotationNumber} بقيمة {quo.TotalAmount:N3} د.ل.");
                RequestWorkflowEngine.Transition(db, request, KitchenRequestStatus.AwaitingContractAndDeposit, User.Identity.GetUserName(), "بانتظار توقيع العقد وسداد دفعة العربون (30%).");
            }

            db.SaveChanges();
            TempData["Success"] = $"تم قبول عرض السعر بنجاح. الطلب الآن بانتظار توقيع العقد ودفع العربون.";
            return RedirectToAction("Details", "Requests", new { id = quo.KitchenRequestId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
