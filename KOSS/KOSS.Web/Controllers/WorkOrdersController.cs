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
    public class WorkOrdersController : Controller
    {
        private readonly KossDbContext db = new KossDbContext();

        // ──────────────────────────────────────────────
        //  GET: /WorkOrders  -  قائمة أوامر التنفيذ
        // ──────────────────────────────────────────────
        public ActionResult Index(WorkOrderStatus? status)
        {
            var query = db.WorkOrders
                .Include(w => w.KitchenRequest)
                .Include(w => w.KitchenRequest.Customer)
                .Include(w => w.Contract)
                .Include(w => w.MaterialRequirements)
                .Include(w => w.Tasks)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(w => w.Status == status.Value);

            var list = query.OrderByDescending(w => w.CreatedAt).ToList();
            ViewBag.Status = status;
            return View(list);
        }

        // ──────────────────────────────────────────────
        //  GET: /WorkOrders/Details/5  -  تفاصيل أمر التنفيذ وقائمة الـ BOM
        // ──────────────────────────────────────────────
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var wo = db.WorkOrders
                .Include(w => w.KitchenRequest)
                .Include(w => w.KitchenRequest.Customer)
                .Include(w => w.Contract)
                .Include(w => w.MaterialRequirements)
                .Include(w => w.StockIssues.Select(si => si.Items.Select(sii => sii.ItemMaster)))
                .Include(w => w.Tasks)
                .Include(w => w.QualityChecks.Select(qc => qc.SnagItems))
                .Include(w => w.InstallationOrders)
                .Include(w => w.HandoverDocuments)
                .FirstOrDefault(w => w.Id == id.Value);

            if (wo == null) return HttpNotFound();

            ViewBag.Warehouses = new SelectList(db.Warehouses.Where(w => w.IsActive).ToList(), "Id", "Name");
            ViewBag.ItemMasters = db.ItemMasters.Where(i => i.IsActive).OrderBy(i => i.Category).ThenBy(i => i.Name).ToList();

            return View(wo);
        }

        // ──────────────────────────────────────────────
        //  POST: /WorkOrders/AddMaterialRequirement  -  إضافة بند لقائمة الـ BOM
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddMaterialRequirement(int workOrderId, int itemMasterId, decimal quantity)
        {
            var item = db.ItemMasters.Find(itemMasterId);
            if (item == null || quantity <= 0)
            {
                TempData["Error"] = "يرجى اختيار الصنف وتحديد كمية صحيحة.";
                return RedirectToAction("Details", new { id = workOrderId });
            }

            var mat = new MaterialRequirement
            {
                WorkOrderId = workOrderId,
                ItemCode = item.ItemCode,
                ItemName = item.Name,
                Category = item.Category,
                Unit = item.Unit,
                QuantityRequired = quantity,
                EstimatedUnitCost = item.StandardCost
            };

            db.MaterialRequirements.Add(mat);
            db.SaveChanges();

            TempData["Success"] = $"تم إضافة الصنف [{item.Name}] بكمية {quantity} {item.Unit} إلى قائمة المواد.";
            return RedirectToAction("Details", new { id = workOrderId });
        }

        // ──────────────────────────────────────────────
        //  POST: /WorkOrders/StartManufacturing  -  بدء التصنيع بالمصنع
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StartManufacturing(int workOrderId)
        {
            var wo = db.WorkOrders.Include(w => w.KitchenRequest).Include(w => w.Tasks).FirstOrDefault(w => w.Id == workOrderId);
            if (wo == null) return HttpNotFound();

            wo.Status = WorkOrderStatus.Manufacturing;

            // إنشاء مهام التصنيع الأساسية إن لم تكن موجودة
            if (!wo.Tasks.Any())
            {
                wo.Tasks.Add(new ManufacturingTask { TaskName = "1. تقطيع الألواح الخشبية (CNC / Sizing)", Status = "InProgress", StartedAt = DateTime.Now });
                wo.Tasks.Add(new ManufacturingTask { TaskName = "2. شريط حواف PVC وحماية الأطراف", Status = "Pending" });
                wo.Tasks.Add(new ManufacturingTask { TaskName = "3. تجميع هياكل الخزائن (Carcass Assembly)", Status = "Pending" });
                wo.Tasks.Add(new ManufacturingTask { TaskName = "4. تركيب المفصلات وسكك الأدراج والإكسسوارات", Status = "Pending" });
                wo.Tasks.Add(new ManufacturingTask { TaskName = "5. التغليف وتجهيز النقل", Status = "Pending" });
            }

            var request = wo.KitchenRequest;
            if (request != null)
            {
                RequestWorkflowEngine.Transition(db, request, KitchenRequestStatus.InManufacturing, User.Identity.GetUserName(), $"بدء تصنيع المطبخ بالمصنع بموجب أمر التشغيل #{wo.OrderNumber}.");
            }

            db.SaveChanges();
            TempData["Success"] = "تم بدء مرحلة التصنيع بالمصنع وتوليد مراحل الإنتاج بنجاح!";
            return RedirectToAction("Details", new { id = workOrderId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
