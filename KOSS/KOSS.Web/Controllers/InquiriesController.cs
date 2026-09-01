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
    public class InquiriesController : Controller
    {
        private readonly KossDbContext db = new KossDbContext();

        // ──────────────────────────────────────────────
        //  GET: /Inquiries  -  قائمة الاستفسارات والفرص البيعية
        // ──────────────────────────────────────────────
        public ActionResult Index(InquiryStatus? status, string search)
        {
            var query = db.CustomerInquiries
                .Include(i => i.Customer)
                .Include(i => i.ConvertedKitchenRequest)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(i => i.Status == status.Value);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(i => i.Customer.Name.Contains(search) || i.Customer.Phone.Contains(search) || i.Location.Contains(search));

            var list = query.OrderByDescending(i => i.CreatedAt).ToList();
            ViewBag.Status = status;
            ViewBag.Search = search;
            return View(list);
        }

        // ──────────────────────────────────────────────
        //  GET: /Inquiries/Create
        // ──────────────────────────────────────────────
        public ActionResult Create(int? customerId)
        {
            ViewBag.Customers = new SelectList(db.Customers.OrderBy(c => c.Name).ToList(), "Id", "Name", customerId);
            return View(new CustomerInquiry { CustomerId = customerId ?? 0 });
        }

        // ──────────────────────────────────────────────
        //  POST: /Inquiries/Create
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(CustomerInquiry inquiry)
        {
            if (inquiry.CustomerId <= 0)
                ModelState.AddModelError("CustomerId", "يرجى اختيار العميل.");

            if (!ModelState.IsValid)
            {
                ViewBag.Customers = new SelectList(db.Customers.OrderBy(c => c.Name).ToList(), "Id", "Name", inquiry.CustomerId);
                return View(inquiry);
            }

            inquiry.CreatedAt = DateTime.Now;
            inquiry.CreatedBy = User.Identity.GetUserName();
            db.CustomerInquiries.Add(inquiry);
            db.SaveChanges();

            TempData["Success"] = "تم تسجيل الاستفسار بنجاح.";
            return RedirectToAction("Index");
        }

        // ──────────────────────────────────────────────
        //  POST: /Inquiries/ConvertToRequest  -  تحويل الاستفسار إلى طلب مطبخ رسمي
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConvertToRequest(int inquiryId)
        {
            var inquiry = db.CustomerInquiries.Include(i => i.Customer).FirstOrDefault(i => i.Id == inquiryId);
            if (inquiry == null) return HttpNotFound();

            int lastId = db.KitchenRequests.Any() ? db.KitchenRequests.Max(r => r.Id) : 0;
            var request = new KitchenRequest
            {
                RequestNumber = $"REQ-{DateTime.Now.Year}-{(lastId + 1):D5}",
                CustomerId = inquiry.CustomerId,
                Location = !string.IsNullOrEmpty(inquiry.Location) ? inquiry.Location : (inquiry.Customer.Address ?? "طرابلس"),
                LayoutType = inquiry.PreferredLayout ?? KitchenLayoutType.Straight,
                ProjectType = ProjectType.Villa,
                Status = KitchenRequestStatus.RequestOpened,
                Notes = $"تحويل من استفسار #{inquiry.Id}: {inquiry.Notes}",
                TargetDeliveryDate = DateTime.Now.AddDays(30),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CreatedBy = User.Identity.GetUserName()
            };

            db.KitchenRequests.Add(request);
            db.SaveChanges();

            // تحديث حالة الاستفسار
            inquiry.Status = InquiryStatus.ConvertedToRequest;
            inquiry.ConvertedKitchenRequestId = request.Id;

            // تسجيل حركة الانتقال
            RequestWorkflowEngine.Transition(db, request, KitchenRequestStatus.RequestOpened, User.Identity.GetUserName(), $"تحويل من استفسار #{inquiry.Id}");
            db.SaveChanges();

            TempData["Success"] = $"تم تحويل الاستفسار إلى طلب مطبخ رسمي رقم [{request.RequestNumber}] بنجاح!";
            return RedirectToAction("Details", "Requests", new { id = request.Id });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
