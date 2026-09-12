using System;
using System.ComponentModel.DataAnnotations;

namespace KOSS.Web.Models
{
    // ============================================================
    //  إعدادات التسعير المركزية بالسوق الليبي (Pricing Settings)
    // ============================================================
    public class PricingSetting
    {
        public int Id { get; set; }

        // ========================================================
        //  1. أسعار الأمتار الأساسية (دينار ليبي / متر)
        // ========================================================
        [Display(Name = "سعر متر الميلامين / فورميكا (د.ل / م.ط)")]
        public decimal MeterRateMelamine { get; set; } = 750m;

        [Display(Name = "سعر متر الأكريليك عالي اللمعان (د.ل / م.ط)")]
        public decimal MeterRateAcrylic { get; set; } = 1100m;

        [Display(Name = "سعر متر البولي لاك وسوبر مات (د.ل / م.ط)")]
        public decimal MeterRatePolylac { get; set; } = 1350m;

        [Display(Name = "سعر متر زجاج بروفايل ألمنيوم وإضاءة (د.ل / م.ط)")]
        public decimal MeterRateAlumGlass { get; set; } = 1750m;

        [Display(Name = "سعر متر دريسنج روم مفتوح بدون درف (د.ل / م.ط)")]
        public decimal MeterRateOpenDressing { get; set; } = 650m;

        // ========================================================
        //  2. تكلفة خامات الكاركاس الداخلي (د.ل / متر محيطي)
        // ========================================================
        [Display(Name = "تكلفة كابينة MDF أبيض مكسو ميلامين")]
        public decimal CarcassRateWhiteMdf { get; set; } = 95m;

        [Display(Name = "تكلفة كابينة MDF أخضر مقاوم للرطوبة HMR")]
        public decimal CarcassRateGreenHmr { get; set; } = 135m;

        [Display(Name = "تكلفة كابينة خشب كونتر Plywood معزول (للحوض)")]
        public decimal CarcassRatePlywood { get; set; } = 180m;

        [Display(Name = "تكلفة كابينة خشب حبيبي معالج اقتصادي")]
        public decimal CarcassRateChipboard { get; set; } = 70m;

        // ========================================================
        //  3. تكلفة خامات الدرف والواجهات (د.ل / متر مربع)
        // ========================================================
        [Display(Name = "تكلفة درف ميلامين / فورميكا (د.ل / م²)")]
        public decimal DoorRateMelamine { get; set; } = 90m;

        [Display(Name = "تكلفة درف أكريليك عالي اللمعان (د.ل / م²)")]
        public decimal DoorRateAcrylic { get; set; } = 180m;

        [Display(Name = "تكلفة درف بولي لاك وسوبر مات (د.ل / م²)")]
        public decimal DoorRatePolylac { get; set; } = 230m;

        [Display(Name = "تكلفة درف زجاج بروفايل ألمنيوم (د.ل / م²)")]
        public decimal DoorRateAlumGlass { get; set; } = 320m;

        // ========================================================
        //  4. أسعار بيع الميكانيزم والإكسسوارات الهيدروليكية (د.ل / قطعة)
        // ========================================================
        [Display(Name = "مفصلات عادية")]
        public decimal PriceStandardHinges { get; set; } = 25m;

        [Display(Name = "مفصلات بلوم هيدروليك Soft-Close")]
        public decimal PriceBlumSoftClose { get; set; } = 85m;

        [Display(Name = "رافعة أبواب علوية Blum Aventos HF/HK")]
        public decimal PriceBlumAventos { get; set; } = 420m;

        [Display(Name = "سلة زاوية ذكية Magic Corner / LeMans")]
        public decimal PriceMagicCorner { get; set; } = 850m;

        [Display(Name = "أدراج تاندم بوكس هيدروليك")]
        public decimal PriceTandemBox { get; set; } = 220m;

        [Display(Name = "منظم ساعات ومجوهرات زجاجي مبطن مخمل")]
        public decimal PriceJewelryOrganizer { get; set; } = 260m;

        [Display(Name = "علاقة بناطيل سحب هيدروليكية")]
        public decimal PriceTrouserRack { get; set; } = 150m;

        [Display(Name = "سلة سحب بهارات وزيوت")]
        public decimal PriceSpiceRack { get; set; } = 95m;

        // ========================================================
        //  5. أسعار أسطح العمل والرخام والكوارتز (د.ل / متر طولي)
        // ========================================================
        [Display(Name = "رخام صناعي أكريليك (Solid Surface)")]
        public decimal PriceCountertopArtificial { get; set; } = 450m;

        [Display(Name = "كوارتز ألماني / تركي 93% (Quartz)")]
        public decimal PriceCountertopQuartz { get; set; } = 680m;

        [Display(Name = "بورسلان مضغوط عالي التحمل (Dekton)")]
        public decimal PriceCountertopDekton { get; set; } = 950m;

        [Display(Name = "جرانيت طبيعي (جلاكسي دبل بلاك)")]
        public decimal PriceCountertopGranite { get; set; } = 550m;

        // ========================================================
        //  6. إضافات الإنارة والمقابض وهامش الربح
        // ========================================================
        [Display(Name = "سعر إضافة إنارة LED مخفية للعلبة")]
        public decimal PriceLedAddon { get; set; } = 65m;

        [Display(Name = "سعر إضافة بروفايل غولا Gola بدون مقبض للعلبة")]
        public decimal PriceGolaAddon { get; set; } = 50m;

        [Display(Name = "نسبة هامش الربح المعياري العام (%)")]
        public decimal ProfitMarginPercentage { get; set; } = 40.0m;

        // سجل التعديل
        public DateTime LastUpdatedAt { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string UpdatedBy { get; set; } = "الإدارة المالية";
    }
}
