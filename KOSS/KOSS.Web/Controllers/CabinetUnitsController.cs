using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KOSS.Web.Models;
using KOSS.Web.Helpers;

namespace KOSS.Web.Controllers
{
    public class CabinetUnitsController : Controller
    {
        private readonly AppDbContext _context;

        public CabinetUnitsController(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        //  إضافة علبة نمطية جديدة لمشروع (Add Box)
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBox(CabinetUnit unit)
        {
            var request = await _context.KitchenRequests
                .Include(r => r.CabinetUnits)
                .FirstOrDefaultAsync(r => r.Id == unit.KitchenRequestId);

            if (request == null) return NotFound("طلب المشروع غير موجود.");

            if (string.IsNullOrWhiteSpace(unit.BoxCode))
            {
                int count = request.CabinetUnits.Count + 1;
                unit.BoxCode = $"BOX-{count:D2}";
            }

            if (string.IsNullOrWhiteSpace(unit.Name))
            {
                unit.Name = unit.Category.ToArabic();
            }

            // احتساب التكلفة وسعر البيع آلياً
            var (cost, price) = LibyanPricingEngine.CalculateBoxCostAndPrice(unit);
            unit.ManufacturingCost = cost;
            unit.SellingPrice = price;
            unit.CreatedBy = User?.Identity?.Name ?? "المشرف الفني";

            _context.CabinetUnits.Add(unit);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"تمت إضافة العلبة [{unit.BoxCode} - {unit.Name}] بنجاح، بتكلفة {unit.ManufacturingCost:N0} د.ل وسعر بيع {unit.SellingPrice:N0} د.ل.";
            return RedirectToAction("Details", "Requests", new { id = unit.KitchenRequestId, tab = "boxes" });
        }

        // ============================================================
        //  حذف علبة من المشروع (Delete Box)
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBox(int id)
        {
            var unit = await _context.CabinetUnits.FindAsync(id);
            if (unit == null) return NotFound("العلبة غير موجودة.");

            int reqId = unit.KitchenRequestId;
            string boxCode = unit.BoxCode;

            _context.CabinetUnits.Remove(unit);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"تم حذف العلبة [{boxCode}] بنجاح.";
            return RedirectToAction("Details", "Requests", new { id = reqId, tab = "boxes" });
        }

        // ============================================================
        //  توليد العلب النمطية تلقائياً بنقرة واحدة (Auto-Generate Template)
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateStandardBoxes(int requestId)
        {
            var request = await _context.KitchenRequests
                .Include(r => r.CabinetUnits)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null) return NotFound("طلب المشروع غير موجود.");

            var templateBoxes = LibyanPricingEngine.GenerateDefaultTemplateBoxes(requestId, request.Category);
            _context.CabinetUnits.AddRange(templateBoxes);
            await _context.SaveChangesAsync();

            string catName = request.Category == CarpentryCategory.DressingRoom ? "حجرة ملابس (Dressing Room)" : "مطبخ حديث";
            TempData["SuccessMessage"] = $"تم توليد {templateBoxes.Count} علب نمطية معتمدة لـ [{catName}] بنجاح مع احتساب تكاليفها وإكسسواراتها.";

            return RedirectToAction("Details", "Requests", new { id = requestId, tab = "boxes" });
        }
    }
}
