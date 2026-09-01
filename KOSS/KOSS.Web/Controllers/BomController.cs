using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KOSS.Web.Models;
using KOSS.Web.Helpers;

namespace KOSS.Web.Controllers
{
    [Authorize]
    public class BomController : Controller
    {
        private readonly KossDbContext db = new KossDbContext();

        // ──────────────────────────────────────────────
        //  GET: /Bom  -  قائمة أوامر الشراء
        // ──────────────────────────────────────────────
        public ActionResult Index()
        {
            var pos = db.PurchaseOrders
                .Include("Contract")
                .Include("Contract.Client")
                .Include("BomItems")
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
            return View(pos);
        }

        // ──────────────────────────────────────────────
        //  GET: /Bom/Create/contractId  -  إنشاء قائمة مواد
        // ──────────────────────────────────────────────
        [Authorize(Roles = "Designer,FactoryManager,Executive")]
        public ActionResult Create(int? contractId)
        {
            if (contractId == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var contract = db.Contracts
                .Include("Client")
                .Include("Units")
                .FirstOrDefault(c => c.Id == contractId);
            if (contract == null) return HttpNotFound();

            // حساب اللوح الخشبي المطلوب
            decimal totalArea = contract.Units.Sum(u => u.TotalArea);
            ViewBag.NestingResult = NestingCalculator.Calculate(totalArea);
            ViewBag.Contract      = contract;

            return View(new PurchaseOrder { ContractId = contractId.Value });
        }

        // ──────────────────────────────────────────────
        //  POST: /Bom/Create
        // ──────────────────────────────────────────────
        [HttpPost, Authorize(Roles = "Designer,FactoryManager,Executive"), ValidateAntiForgeryToken]
        public ActionResult Create(PurchaseOrder po)
        {
            if (!ModelState.IsValid) return View(po);

            // إنشاء رقم أمر الشراء
            int lastId   = db.PurchaseOrders.Any() ? db.PurchaseOrders.Max(p => p.Id) : 0;
            po.PoNumber  = $"PO-{DateTime.Now.Year}-{(lastId + 1):D5}";
            po.CreatedAt = DateTime.Now;
            po.CreatedBy = User.Identity.GetUserName();
            po.Status    = PurchaseOrderStatus.Draft;

            db.PurchaseOrders.Add(po);
            db.SaveChanges();

            TempData["Success"] = $"تم إنشاء أمر الشراء رقم {po.PoNumber}.";
            return RedirectToAction("Details", new { id = po.Id });
        }

        // ──────────────────────────────────────────────
        //  GET: /Bom/Details/5
        // ──────────────────────────────────────────────
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var po = db.PurchaseOrders
                .Include("Contract")
                .Include("Contract.Client")
                .Include("Contract.Units")
                .Include("BomItems")
                .FirstOrDefault(p => p.Id == id);
            if (po == null) return HttpNotFound();

            // حساب نتيجة التداخل
            decimal totalArea     = po.Contract?.Units?.Sum(u => u.TotalArea) ?? 0;
            ViewBag.NestingResult = NestingCalculator.Calculate(totalArea);

            return View(po);
        }

        // ──────────────────────────────────────────────
        //  POST: /Bom/SendToWarehouse/5
        // ──────────────────────────────────────────────
        [HttpPost, Authorize(Roles = "Designer,FactoryManager,Executive")]
        public ActionResult SendToWarehouse(int id)
        {
            var po = db.PurchaseOrders.Find(id);
            if (po == null) return HttpNotFound();

            po.Status             = PurchaseOrderStatus.SentToWarehouse;
            po.SentToWarehouseAt  = DateTime.Now;
            db.SaveChanges();

            TempData["Success"] = $"تم إرسال أمر الشراء {po.PoNumber} إلى المستودع.";
            return RedirectToAction("Details", new { id });
        }

        // ──────────────────────────────────────────────
        //  POST: /Bom/IssueToFactory/5
        // ──────────────────────────────────────────────
        [HttpPost, Authorize(Roles = "FactoryManager,Executive")]
        public ActionResult IssueToFactory(int id)
        {
            var po = db.PurchaseOrders
                .Include("BomItems")
                .Include("Contract")
                .FirstOrDefault(p => p.Id == id);
            if (po == null) return HttpNotFound();

            po.Status             = PurchaseOrderStatus.IssuedToFactory;
            po.IssuedToFactoryAt  = DateTime.Now;

            foreach (var item in po.BomItems)
                item.IssuedToFactory = true;

            // تحديث حالة العقد → تحت التصنيع
            if (po.Contract != null)
            {
                po.Contract.Status    = ContractStatus.UnderProduction;
                po.Contract.UpdatedAt = DateTime.Now;
            }
            db.SaveChanges();

            TempData["Success"] = $"تم صرف أمر الشراء {po.PoNumber} للمصنع. بدأ التصنيع!";
            return RedirectToAction("Details", new { id });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
