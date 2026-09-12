using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KOSS.Web.Models;

namespace KOSS.Web.Controllers
{
    public class PricingSettingsController : Controller
    {
        private readonly AppDbContext _context;

        public PricingSettingsController(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        //  عرض شاشة إعدادات التسعير المركزية (Index)
        // ============================================================
        public async Task<IActionResult> Index()
        {
            var settings = await _context.PricingSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new PricingSetting();
                _context.PricingSettings.Add(settings);
                await _context.SaveChangesAsync();
            }

            return View(settings);
        }

        // ============================================================
        //  تحديث وحفظ أسعار المنظومة المركزية (Update)
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PricingSetting model)
        {
            var current = await _context.PricingSettings.FirstOrDefaultAsync();
            if (current == null)
            {
                current = new PricingSetting();
                _context.PricingSettings.Add(current);
            }

            // 1. أسعار الأمتار
            current.MeterRateMelamine = model.MeterRateMelamine;
            current.MeterRateAcrylic = model.MeterRateAcrylic;
            current.MeterRatePolylac = model.MeterRatePolylac;
            current.MeterRateAlumGlass = model.MeterRateAlumGlass;
            current.MeterRateOpenDressing = model.MeterRateOpenDressing;

            // 2. تكاليف الكاركاس
            current.CarcassRateWhiteMdf = model.CarcassRateWhiteMdf;
            current.CarcassRateGreenHmr = model.CarcassRateGreenHmr;
            current.CarcassRatePlywood = model.CarcassRatePlywood;
            current.CarcassRateChipboard = model.CarcassRateChipboard;

            // 3. تكاليف الدرف
            current.DoorRateMelamine = model.DoorRateMelamine;
            current.DoorRateAcrylic = model.DoorRateAcrylic;
            current.DoorRatePolylac = model.DoorRatePolylac;
            current.DoorRateAlumGlass = model.DoorRateAlumGlass;

            // 4. أسعار الميكانيزم والإكسسوارات
            current.PriceStandardHinges = model.PriceStandardHinges;
            current.PriceBlumSoftClose = model.PriceBlumSoftClose;
            current.PriceBlumAventos = model.PriceBlumAventos;
            current.PriceMagicCorner = model.PriceMagicCorner;
            current.PriceTandemBox = model.PriceTandemBox;
            current.PriceJewelryOrganizer = model.PriceJewelryOrganizer;
            current.PriceTrouserRack = model.PriceTrouserRack;
            current.PriceSpiceRack = model.PriceSpiceRack;

            // 5. أسعار أسطح العمل
            current.PriceCountertopArtificial = model.PriceCountertopArtificial;
            current.PriceCountertopQuartz = model.PriceCountertopQuartz;
            current.PriceCountertopDekton = model.PriceCountertopDekton;
            current.PriceCountertopGranite = model.PriceCountertopGranite;

            // 6. الإضافات وهامش الربح
            current.PriceLedAddon = model.PriceLedAddon;
            current.PriceGolaAddon = model.PriceGolaAddon;
            current.ProfitMarginPercentage = model.ProfitMarginPercentage;

            current.LastUpdatedAt = DateTime.Now;
            current.UpdatedBy = User?.Identity?.Name ?? "الإدارة التنفيذية";

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "تم حفظ وتحديث إعدادات أسعار المنظومة بنجاح، وتنعكس فورياً على كافة الحسابات وعروض الأسعار.";
            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        //  استعادة الأسعار القياسية للسوق الليبي (ResetToDefaults)
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetToDefaults()
        {
            var current = await _context.PricingSettings.FirstOrDefaultAsync();
            if (current != null)
            {
                var def = new PricingSetting();

                current.MeterRateMelamine = def.MeterRateMelamine;
                current.MeterRateAcrylic = def.MeterRateAcrylic;
                current.MeterRatePolylac = def.MeterRatePolylac;
                current.MeterRateAlumGlass = def.MeterRateAlumGlass;
                current.MeterRateOpenDressing = def.MeterRateOpenDressing;

                current.CarcassRateWhiteMdf = def.CarcassRateWhiteMdf;
                current.CarcassRateGreenHmr = def.CarcassRateGreenHmr;
                current.CarcassRatePlywood = def.CarcassRatePlywood;
                current.CarcassRateChipboard = def.CarcassRateChipboard;

                current.DoorRateMelamine = def.DoorRateMelamine;
                current.DoorRateAcrylic = def.DoorRateAcrylic;
                current.DoorRatePolylac = def.DoorRatePolylac;
                current.DoorRateAlumGlass = def.DoorRateAlumGlass;

                current.PriceStandardHinges = def.PriceStandardHinges;
                current.PriceBlumSoftClose = def.PriceBlumSoftClose;
                current.PriceBlumAventos = def.PriceBlumAventos;
                current.PriceMagicCorner = def.PriceMagicCorner;
                current.PriceTandemBox = def.PriceTandemBox;
                current.PriceJewelryOrganizer = def.PriceJewelryOrganizer;
                current.PriceTrouserRack = def.PriceTrouserRack;
                current.PriceSpiceRack = def.PriceSpiceRack;

                current.PriceCountertopArtificial = def.PriceCountertopArtificial;
                current.PriceCountertopQuartz = def.PriceCountertopQuartz;
                current.PriceCountertopDekton = def.PriceCountertopDekton;
                current.PriceCountertopGranite = def.PriceCountertopGranite;

                current.PriceLedAddon = def.PriceLedAddon;
                current.PriceGolaAddon = def.PriceGolaAddon;
                current.ProfitMarginPercentage = def.ProfitMarginPercentage;

                current.LastUpdatedAt = DateTime.Now;
                current.UpdatedBy = "استعادة الإعدادات الافتراضية";

                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "تمت استعادة الأسعار القياسية الافتراضية للسوق الليبي بنجاح.";
            return RedirectToAction(nameof(Index));
        }
    }
}
