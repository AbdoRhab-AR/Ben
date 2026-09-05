using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOSS.Web.Models
{
    // ============================================================
    //  فئات العلب والوحدات النمطية (Cabinet / Unit Categories)
    // ============================================================
    public enum CabinetUnitCategory
    {
        [Display(Name = "علبة سفلية - مطبخ (Base Unit)")]
        BaseCabinet = 1,

        [Display(Name = "علبة علوية - مطبخ (Wall Unit)")]
        WallCabinet = 2,

        [Display(Name = "برج طولي - فرن / بانتري (Tall Tower)")]
        TallCabinet = 3,

        [Display(Name = "علبة جزيرة وسطية (Island Unit)")]
        IslandCabinet = 4,

        [Display(Name = "علب سقفية علوية (Loft Cabinets)")]
        LoftCabinet = 5,

        [Display(Name = "باكية تعليق طويل - دريسنج روم (Long Hanging)")]
        DressingLongHang = 6,

        [Display(Name = "باكية تعليق قصير - دريسنج روم (Short Hanging)")]
        DressingShortHang = 7,

        [Display(Name = "باكية أدراج وساعات ومجوهرات (Jewelry & Drawers)")]
        DressingDrawersAndJewelry = 8,

        [Display(Name = "باكية أرفف ملابس وأحذية (Shelves & Shoes)")]
        DressingShelvesAndShoes = 9,

        [Display(Name = "جزيرة دريسنج روم وسطية (Dressing Island)")]
        DressingIsland = 10
    }

    // ============================================================
    //  خامات العلبة الداخلية (Carcass Material)
    // ============================================================
    public enum CarcassMaterial
    {
        [Display(Name = "MDF أبيض تركي مكسو ميلامين")]
        WhiteMelamineMdf = 1,

        [Display(Name = "MDF أخضر تركي مقاوم للرطوبة (HMR)")]
        MoistureResistantGreenHmr = 2,

        [Display(Name = "خشب كونتر / Plywood معزول (مقاوم للمياه للحوض)")]
        MarinePlywoodUnderSink = 3,

        [Display(Name = "خشب حبيبي معالج اقتصادي (Chipboard)")]
        Chipboard = 4
    }

    // ============================================================
    //  خامات الواجهة والدرفة (Door & Front Finish)
    // ============================================================
    public enum FrontDoorType
    {
        [Display(Name = "أكريليك عالي اللمعان (High Gloss Acrylic)")]
        HighGlossAcrylic = 1,

        [Display(Name = "بولي لاك وسوبر مات (Polylac / Super Matt)")]
        PolylacSuperMatt = 2,

        [Display(Name = "ميلامين / فورميكا اقتصادي (Melamine / HPL)")]
        MelamineFormica = 3,

        [Display(Name = "زجاج سموكي عاكس بإطار بروفايل ألمنيوم")]
        AluminiumFrameGlass = 4,

        [Display(Name = "مفتوحة بدون درف (Open Concept - Walk-in)")]
        OpenWalkInNoDoors = 5
    }

    // ============================================================
    //  الإكسسوارات والميكانيزم الهيدروليكي المضاف
    // ============================================================
    public enum MechanismType
    {
        [Display(Name = "مفصلات عادية")]
        StandardHinges = 1,

        [Display(Name = "مفصلات بلوم / هيدروليك Soft-Close")]
        BlumSoftCloseHinges = 2,

        [Display(Name = "رافعة هيدروليكية مزدوجة (Blum Aventos HF/HK)")]
        BlumAventosDoubleLift = 3,

        [Display(Name = "سلة زاوية ذكية (Magic Corner / LeMans)")]
        MagicCornerOrLeMans = 4,

        [Display(Name = "أدراج تاندم بوكس هيدروليك (TandemBox Drawers)")]
        TandemBoxDrawers = 5,

        [Display(Name = "منظم ساعات ومجوهرات مبطن مخمل وزجاج")]
        VelvetJewelryOrganizer = 6,

        [Display(Name = "علاقة بناطيل سحب هيدروليكية")]
        PullOutTrouserRack = 7,

        [Display(Name = "سلة سحب بهارات وزيوت (Spice Pull-out)")]
        SpiceRackPullOut = 8
    }

    // ============================================================
    //  نموذج العلبة / الباكية النمطية (CabinetUnit)
    // ============================================================
    public class CabinetUnit
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "طلب المشروع المركزي")]
        public int KitchenRequestId { get; set; }

        [Required, StringLength(30)]
        [Display(Name = "كود العلبة (مثال: B-SINK-90)")]
        public string BoxCode { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "اسم العلبة ومواصفاتها")]
        public string Name { get; set; }

        [Display(Name = "فئة ونوع العلبة")]
        public CabinetUnitCategory Category { get; set; } = CabinetUnitCategory.BaseCabinet;

        [Display(Name = "العرض (سم)")]
        public decimal WidthCm { get; set; } = 60;

        [Display(Name = "الارتفاع (سم)")]
        public decimal HeightCm { get; set; } = 85;

        [Display(Name = "العمق (سم)")]
        public decimal DepthCm { get; set; } = 60;

        [Display(Name = "خامة العلبة الداخلية (الكاركاس)")]
        public CarcassMaterial Carcass { get; set; } = CarcassMaterial.WhiteMelamineMdf;

        [Display(Name = "خامة الواجهة والدرفة")]
        public FrontDoorType DoorType { get; set; } = FrontDoorType.HighGlossAcrylic;

        [Display(Name = "الإكسسوار أو الميكانيزم المضاف")]
        public MechanismType Mechanism { get; set; } = MechanismType.BlumSoftCloseHinges;

        [Display(Name = "هل مدمج بها إضاءة LED مخفية؟")]
        public bool HasLedLighting { get; set; } = false;

        [Display(Name = "هل تستخدم بروفايل ألمنيوم غولا (بدون مقبض)؟")]
        public bool HasGolaProfile { get; set; } = true;

        [Display(Name = "تكلفة التصنيع التقديرية (د.ل)")]
        public decimal ManufacturingCost { get; set; } = 0;

        [Display(Name = "سعر البيع المحسوب للعلبة (د.ل)")]
        public decimal SellingPrice { get; set; } = 0;

        [Display(Name = "ملاحظات الفني / التصنيع")]
        [StringLength(500)]
        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }

        // العلاقة
        public virtual KitchenRequest KitchenRequest { get; set; }
    }
}
