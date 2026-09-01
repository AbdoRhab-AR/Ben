using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KOSS.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KOSS.Web.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly AppDbContext _context;

        public InventoryController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _context.ItemMasters.Include(i => i.StockItems).OrderBy(i => i.Category).ThenBy(i => i.Name).ToListAsync();
            return View(items);
        }

        public async Task<IActionResult> StockIssues()
        {
            var issues = await _context.StockIssues
                .Include(s => s.KitchenRequest)
                    .ThenInclude(r => r.Customer)
                .Include(s => s.Items)
                .OrderByDescending(s => s.IssuedAt)
                .ToListAsync();

            return View(issues);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IssueToProject(int kitchenRequestId, int? workOrderId, int itemMasterId, decimal quantity, string notes)
        {
            var item = await _context.ItemMasters.FindAsync(itemMasterId);
            if (item == null) return NotFound();

            if (quantity <= 0)
            {
                TempData["Error"] = "يرجى تحديد كمية صحيحة للصرف.";
                return RedirectToAction("Details", "Requests", new { id = kitchenRequestId });
            }

            var defaultWarehouse = await _context.Warehouses.FirstOrDefaultAsync() ?? new Warehouse { Name = "المستودع الرئيسي", Code = "WH-01" };
            if (defaultWarehouse.Id == 0)
            {
                _context.Warehouses.Add(defaultWarehouse);
                await _context.SaveChangesAsync();
            }

            var issue = new StockIssue
            {
                KitchenRequestId = kitchenRequestId,
                WorkOrderId = workOrderId,
                WarehouseId = defaultWarehouse.Id,
                IssueNumber = $"ISS-{DateTime.Now.Year}-{new Random().Next(1000, 9999)}",
                IssuedAt = DateTime.Now,
                RecipientName = User.Identity?.Name ?? "StoreKeeper",
                TotalCost = quantity * item.StandardCost,
                Notes = notes
            };

            _context.StockIssues.Add(issue);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"تم صرف ({quantity} {item.Unit}) من ({item.Name}) وتحميل التكلفة ({issue.TotalCost:N3} د.ل) على المشروع بنجاح.";
            return RedirectToAction("Details", "Requests", new { id = kitchenRequestId });
        }
    }
}
