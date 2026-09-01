using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KOSS.Web.Models;

namespace KOSS.Web.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly KossDbContext db = new KossDbContext();

        // ──────────────────────────────────────────────
        //  GET: /Inventory  -  أرصدة المخزون والأصناف
        // ──────────────────────────────────────────────
        public ActionResult Index(string search, string category, int? warehouseId)
        {
            var query = db.StockItems
                .Include(s => s.Warehouse)
                .Include(s => s.ItemMaster)
                .AsQueryable();

            if (warehouseId.HasValue)
                query = query.Where(s => s.WarehouseId == warehouseId.Value);

            if (!string.IsNullOrEmpty(category))
                query = query.Where(s => s.ItemMaster.Category == category);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(s => s.ItemMaster.Name.Contains(search) || s.ItemMaster.ItemCode.Contains(search));

            var list = query.OrderBy(s => s.ItemMaster.Category).ThenBy(s => s.ItemMaster.Name).ToList();

            ViewBag.Warehouses = new SelectList(db.Warehouses.Where(w => w.IsActive).ToList(), "Id", "Name", warehouseId);
            ViewBag.Categories = db.ItemMasters.Select(i => i.Category).Distinct().ToList();
            ViewBag.Search = search;
            ViewBag.Category = category;

            return View(list);
        }

        // ──────────────────────────────────────────────
        //  GET: /Inventory/StockIssues  -  سندات الصرف للمشاريع
        // ──────────────────────────────────────────────
        public ActionResult StockIssues()
        {
            var issues = db.StockIssues
                .Include(s => s.KitchenRequest)
                .Include(s => s.KitchenRequest.Customer)
                .Include(s => s.Warehouse)
                .Include(s => s.Items.Select(i => i.ItemMaster))
                .OrderByDescending(s => s.IssuedAt)
                .ToList();

            return View(issues);
        }

        // ──────────────────────────────────────────────
        //  POST: /Inventory/CreateIssue  -  إصدار سند صرف مخزني لمشروع مطبخ
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CreateIssue(int kitchenRequestId, int? workOrderId, int warehouseId, int itemMasterId, decimal quantity, string recipientName, string notes)
        {
            var item = db.ItemMasters.Find(itemMasterId);
            var warehouse = db.Warehouses.Find(warehouseId);
            var request = db.KitchenRequests.Find(kitchenRequestId);

            if (item == null || warehouse == null || request == null || quantity <= 0)
            {
                TempData["Error"] = "بيانات سند الصرف غير مكتملة أو الكمية غير صالحة.";
                return RedirectToAction("Details", "Requests", new { id = kitchenRequestId });
            }

            // فحص رصيد المخزن
            var stockItem = db.StockItems.FirstOrDefault(s => s.WarehouseId == warehouseId && s.ItemMasterId == itemMasterId);
            if (stockItem == null || stockItem.PhysicalQuantity < quantity)
            {
                decimal current = stockItem != null ? stockItem.PhysicalQuantity : 0;
                TempData["Error"] = $"عفواً، الكمية المتوفرة بالمخزن ({current} {item.Unit}) غير كافية لصرف ({quantity} {item.Unit}).";
                return RedirectToAction("Details", "Requests", new { id = kitchenRequestId });
            }

            int lastId = db.StockIssues.Any() ? db.StockIssues.Max(s => s.Id) : 0;
            string issueNo = $"ISS-{DateTime.Now.Year}-{(lastId + 1):D5}";

            decimal unitCost = stockItem.WeightedAverageCost > 0 ? stockItem.WeightedAverageCost : item.StandardCost;
            decimal totalCost = quantity * unitCost;

            var issue = new StockIssue
            {
                KitchenRequestId = kitchenRequestId,
                WorkOrderId = workOrderId,
                WarehouseId = warehouseId,
                IssueNumber = issueNo,
                IssuedAt = DateTime.Now,
                RecipientName = recipientName ?? "فني المصنع",
                TotalCost = totalCost,
                ApprovedBy = User.Identity.GetUserName(),
                Notes = notes
            };

            issue.Items.Add(new StockIssueItem
            {
                ItemMasterId = itemMasterId,
                QuantityIssued = quantity,
                UnitCost = unitCost
            });

            db.StockIssues.Add(issue);

            // خصم الكمية من المخزون
            stockItem.PhysicalQuantity -= quantity;
            stockItem.LastUpdated = DateTime.Now;

            // تسجيل في سجل الحركات المخزنية
            db.StockTransactions.Add(new StockTransaction
            {
                WarehouseId = warehouseId,
                ItemMasterId = itemMasterId,
                TransactionType = StockTransactionType.IssueToProject,
                ReferenceNumber = issueNo,
                KitchenRequestId = kitchenRequestId,
                OutQuantity = quantity,
                UnitCost = unitCost,
                TransactionDate = DateTime.Now,
                CreatedBy = User.Identity.GetUserName()
            });

            // تحديث كمية الصرف في قائمة الـ BOM إن وجدت
            if (workOrderId.HasValue)
            {
                var bom = db.MaterialRequirements.FirstOrDefault(m => m.WorkOrderId == workOrderId.Value && m.ItemCode == item.ItemCode);
                if (bom != null)
                {
                    bom.QuantityIssued += quantity;
                }
            }

            db.SaveChanges();
            TempData["Success"] = $"تم إصدار سند الصرف #{issueNo} بنجاح وخصم ({quantity} {item.Unit}) بقيمة ({totalCost:N3} د.ل) وإضافتها لتكاليف المشروع.";
            return RedirectToAction("Details", "Requests", new { id = kitchenRequestId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
