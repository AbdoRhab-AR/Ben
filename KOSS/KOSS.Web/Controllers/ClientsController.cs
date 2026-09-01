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
    public class ClientsController : Controller
    {
        private readonly KossDbContext db = new KossDbContext();

        // ──────────────────────────────────────────────
        //  GET: /Clients  -  قائمة العملاء
        // ──────────────────────────────────────────────
        public ActionResult Index(string search, ClientStatus? status, int page = 1)
        {
            int pageSize = 15;
            var query = db.Clients.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(c =>
                    c.Name.Contains(search) ||
                    c.Phone.Contains(search) ||
                    c.Address.Contains(search));

            if (status.HasValue)
                query = query.Where(c => c.Status == status.Value);

            ViewBag.Search  = search;
            ViewBag.Status  = status;
            ViewBag.Total   = query.Count();
            ViewBag.Page    = page;
            ViewBag.Pages   = (int)Math.Ceiling(query.Count() / (double)pageSize);

            var clients = query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return View(clients);
        }

        // ──────────────────────────────────────────────
        //  GET: /Clients/Create  -  تسجيل عميل جديد
        // ──────────────────────────────────────────────
        [Authorize(Roles = "SalesStaff,Executive")]
        public ActionResult Create() => View();

        // ──────────────────────────────────────────────
        //  POST: /Clients/Create
        // ──────────────────────────────────────────────
        [HttpPost, Authorize(Roles = "SalesStaff,Executive"), ValidateAntiForgeryToken]
        public ActionResult Create(Client client)
        {
            if (!ModelState.IsValid) return View(client);

            client.CreatedAt = DateTime.Now;
            client.CreatedBy = User.Identity.GetUserName();
            db.Clients.Add(client);
            db.SaveChanges();

            TempData["Success"] = $"تم تسجيل العميل '{client.Name}' بنجاح!";

            // إذا مهتم → انتقل مباشرة لإنشاء عقد
            if (client.Status == ClientStatus.Interested)
                return RedirectToAction("Create", "Contracts", new { clientId = client.Id });

            return RedirectToAction("Index");
        }

        // ──────────────────────────────────────────────
        //  GET: /Clients/Details/5  -  ملف العميل الكامل
        // ──────────────────────────────────────────────
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var client = db.Clients
                .Include(c => c.Contracts)
                .Include("Contracts.Units")
                .Include("Contracts.Payments")
                .FirstOrDefault(c => c.Id == id);

            if (client == null) return HttpNotFound();
            return View(client);
        }

        // ──────────────────────────────────────────────
        //  POST: /Clients/SetStatus  -  تغيير الحالة (AJAX)
        // ──────────────────────────────────────────────
        [HttpPost, Authorize(Roles = "SalesStaff,Executive")]
        public ActionResult SetStatus(int id, ClientStatus status)
        {
            var client = db.Clients.Find(id);
            if (client == null)
                return Json(new { success = false, message = "العميل غير موجود" });

            client.Status = status;
            db.SaveChanges();

            return Json(new
            {
                success    = true,
                message    = $"تم تغيير حالة '{client.Name}' إلى '{status}'",
                newStatus  = status.ToString()
            });
        }

        // ──────────────────────────────────────────────
        //  GET: /Clients/Edit/5
        // ──────────────────────────────────────────────
        [Authorize(Roles = "SalesStaff,Executive")]
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var client = db.Clients.Find(id);
            if (client == null) return HttpNotFound();
            return View(client);
        }

        // ──────────────────────────────────────────────
        //  POST: /Clients/Edit
        // ──────────────────────────────────────────────
        [HttpPost, Authorize(Roles = "SalesStaff,Executive"), ValidateAntiForgeryToken]
        public ActionResult Edit(Client client)
        {
            if (!ModelState.IsValid) return View(client);
            db.Entry(client).State = EntityState.Modified;
            db.SaveChanges();
            TempData["Success"] = "تم تحديث بيانات العميل بنجاح.";
            return RedirectToAction("Details", new { id = client.Id });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
