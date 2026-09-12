using System;
using System.Collections.Generic;
using System.Linq;
using KOSS.Web.Models;

namespace KOSS.Web.Helpers
{
    // ============================================================
    //  فروقات الأسعار والسيناريوهات المادية (Price Variance Item)
    // ============================================================
    public class MaterialVarianceOption
    {
        public string MaterialName { get; set; }
        public string Description { get; set; }
        public decimal PricePerMeter { get; set; }
        public decimal TotalProjectEstimatedPrice { get; set; }
        public decimal PriceDifferenceFromBaseline { get; set; }
        public string TierBadge { get; set; } // اقتصادي، الأكثر طلباً، راقي، VIP
    }

    public class MechanismVarianceOption
    {
        public string MechanismName { get; set; }
        public string AppliedTo { get; set; }
        public decimal UnitCost { get; set; }
        public decimal SellingPrice { get; set; }
        public string Advantage { get; set; }
    }

    public class ProjectPricingSummary
    {
        public int KitchenRequestId { get; set; }
        public string RequestNumber { get; set; }
        public CarpentryCategory Category { get; set; }
        public int TotalBoxesCount { get; set; }
        public decimal TotalWidthLinearMeters { get; set; }
        public decimal TotalEstimatedSquareMeters { get; set; }

        public decimal TotalModularBoxesCost { get; set; }
        public decimal TotalModularBoxesSellingPrice { get; set; }

        public decimal RunningMeterEquivalentPrice { get; set; }
        public decimal SquareMeterEquivalentPrice { get; set; }

        public List<MaterialVarianceOption> DoorFinishVariances { get; set; } = new List<MaterialVarianceOption>();
        public List<MechanismVarianceOption> HardwareVariances { get; set; } = new List<MechanismVarianceOption>();
        public List<MaterialVarianceOption> CountertopVariances { get; set; } = new List<MaterialVarianceOption>();
    }

    // ============================================================
    //  المحرك الذكي لحساب تكاليف وعروض أسعار السوق الليبي
    // ============================================================
    public static class LibyanPricingEngine
    {
        // ============================================================
        //  حساب تكلفة وسعر بيع العلبة الواحدة (Modular Box Pricing)
        // ============================================================
        public static (decimal cost, decimal price) CalculateBoxCostAndPrice(CabinetUnit box, PricingSetting settings = null)
        {
            if (box == null) return (0, 0);
            settings ??= new PricingSetting();

            decimal widthMeter = box.WidthCm / 100m;
            decimal heightMeter = box.HeightCm / 100m;
            decimal depthMeter = box.DepthCm / 100m;
            decimal faceAreaM2 = widthMeter * heightMeter;
            decimal perimeterM = (widthMeter * 2 + heightMeter * 2 + depthMeter * 2);

            // 1. تكلفة خامة الكاركاس الداخلي (ديناميكي من الإعدادات)
            decimal carcassRate = box.Carcass switch
            {
                CarcassMaterial.WhiteMelamineMdf => settings.CarcassRateWhiteMdf,
                CarcassMaterial.MoistureResistantGreenHmr => settings.CarcassRateGreenHmr,
                CarcassMaterial.MarinePlywoodUnderSink => settings.CarcassRatePlywood,
                CarcassMaterial.Chipboard => settings.CarcassRateChipboard,
                _ => settings.CarcassRateWhiteMdf
            };
            decimal carcassBaseCost = carcassRate * perimeterM;

            // 2. تكلفة خامة الواجهة / الدرفة (ديناميكي من الإعدادات)
            decimal doorRate = box.DoorType switch
            {
                FrontDoorType.HighGlossAcrylic => settings.DoorRateAcrylic,
                FrontDoorType.PolylacSuperMatt => settings.DoorRatePolylac,
                FrontDoorType.MelamineFormica => settings.DoorRateMelamine,
                FrontDoorType.AluminiumFrameGlass => settings.DoorRateAlumGlass,
                FrontDoorType.OpenWalkInNoDoors => 0m,
                _ => settings.DoorRateMelamine
            };
            decimal doorCost = faceAreaM2 * doorRate;

            // 3. تكلفة الميكانيزم والإكسسوارات (تكلفة الشراء التقديرية ~ 70% من سعر البيع)
            decimal mechanismSellingPrice = box.Mechanism switch
            {
                MechanismType.StandardHinges => settings.PriceStandardHinges,
                MechanismType.BlumSoftCloseHinges => settings.PriceBlumSoftClose,
                MechanismType.BlumAventosDoubleLift => settings.PriceBlumAventos,
                MechanismType.MagicCornerOrLeMans => settings.PriceMagicCorner,
                MechanismType.TandemBoxDrawers => settings.PriceTandemBox,
                MechanismType.VelvetJewelryOrganizer => settings.PriceJewelryOrganizer,
                MechanismType.PullOutTrouserRack => settings.PriceTrouserRack,
                MechanismType.SpiceRackPullOut => settings.PriceSpiceRack,
                _ => settings.PriceStandardHinges
            };
            decimal mechanismCost = Math.Round(mechanismSellingPrice * 0.70m, 2);

            // 4. إضافات الإنارة والبروفايل
            decimal addOnCost = 0m;
            if (box.HasLedLighting) addOnCost += Math.Round(settings.PriceLedAddon * 0.70m, 2);
            if (box.HasGolaProfile) addOnCost += Math.Round(settings.PriceGolaAddon * 0.70m, 2);

            decimal totalCost = Math.Round(carcassBaseCost + doorCost + mechanismCost + addOnCost, 2);

            // هامش ربح معياري ديناميكي من الإعدادات
            decimal marginMultiplier = 1.0m + (settings.ProfitMarginPercentage / 100m);
            decimal sellingPrice = Math.Round(totalCost * marginMultiplier, 0);

            return (totalCost, sellingPrice);
        }

        // ============================================================
        //  توليد ملخص المشروع ومصفوفة مقارنة وفروقات الأسعار
        // ============================================================
        public static ProjectPricingSummary GeneratePricingSummary(KitchenRequest request, PricingSetting settings = null)
        {
            settings ??= new PricingSetting();

            var summary = new ProjectPricingSummary
            {
                KitchenRequestId = request.Id,
                RequestNumber = request.RequestNumber ?? $"REQ-{request.Id}",
                Category = request.Category
            };

            var boxes = request.CabinetUnits?.ToList() ?? new List<CabinetUnit>();
            summary.TotalBoxesCount = boxes.Count;

            // حساب الأمتار الطولية الإجمالية
            if (boxes.Any())
            {
                var mainUnits = boxes.Where(b => b.Category == CabinetUnitCategory.BaseCabinet ||
                                                 b.Category == CabinetUnitCategory.TallCabinet ||
                                                 b.Category == CabinetUnitCategory.DressingLongHang ||
                                                 b.Category == CabinetUnitCategory.DressingShortHang ||
                                                 b.Category == CabinetUnitCategory.DressingDrawersAndJewelry ||
                                                 b.Category == CabinetUnitCategory.DressingShelvesAndShoes).ToList();

                summary.TotalWidthLinearMeters = mainUnits.Any() ? Math.Round(mainUnits.Sum(b => b.WidthCm) / 100m, 2) : 5.0m;
                summary.TotalModularBoxesCost = boxes.Sum(b => b.ManufacturingCost);
                summary.TotalModularBoxesSellingPrice = boxes.Sum(b => b.SellingPrice);
            }
            else
            {
                var sv = request.SiteVisits?.FirstOrDefault(s => s.Status == SiteVisitStatus.Approved) ?? request.SiteVisits?.LastOrDefault();
                summary.TotalWidthLinearMeters = sv != null && (sv.WallLength1Cm + sv.WallLength2Cm) > 0
                    ? Math.Round((sv.WallLength1Cm + sv.WallLength2Cm) / 100m, 2)
                    : 6.0m;
            }

            summary.TotalEstimatedSquareMeters = Math.Round(summary.TotalWidthLinearMeters * 2.4m, 2);

            if (summary.TotalWidthLinearMeters > 0 && summary.TotalModularBoxesSellingPrice > 0)
            {
                summary.RunningMeterEquivalentPrice = Math.Round(summary.TotalModularBoxesSellingPrice / summary.TotalWidthLinearMeters, 0);
            }
            else
            {
                summary.RunningMeterEquivalentPrice = settings.MeterRateAcrylic;
                summary.TotalModularBoxesSellingPrice = summary.RunningMeterEquivalentPrice * summary.TotalWidthLinearMeters;
            }

            summary.SquareMeterEquivalentPrice = summary.TotalEstimatedSquareMeters > 0
                ? Math.Round(summary.TotalModularBoxesSellingPrice / summary.TotalEstimatedSquareMeters, 0)
                : 450m;

            decimal linearM = summary.TotalWidthLinearMeters > 0 ? summary.TotalWidthLinearMeters : 6.0m;
            decimal baselineRate = settings.MeterRateAcrylic;

            // ========================================================
            //  1. مصفوفة مقارنة خامات الدرف والواجهات (ديناميكي من الإعدادات)
            // ========================================================
            summary.DoorFinishVariances = new List<MaterialVarianceOption>
            {
                new MaterialVarianceOption
                {
                    MaterialName = "ميلامين / فورميكا اقتصادي (Melamine / HPL)",
                    Description = "خامة عملية واقتصادية متينة مقاومة للخدوش البسيطة، متوفرة بألوان وأخشاب متعددة.",
                    PricePerMeter = settings.MeterRateMelamine,
                    TotalProjectEstimatedPrice = Math.Round(linearM * settings.MeterRateMelamine, 0),
                    PriceDifferenceFromBaseline = Math.Round((settings.MeterRateMelamine - baselineRate) * linearM, 0),
                    TierBadge = "الخيار الاقتصادي"
                },
                new MaterialVarianceOption
                {
                    MaterialName = "أكريليك عالي اللمعان (High Gloss Acrylic)",
                    Description = "الخامة الأكثر طلباً وشعبية بالسوق الليبي، لمعان زجاجي فائق 95 Gloss ومقاومة للبخار والرطوبة.",
                    PricePerMeter = settings.MeterRateAcrylic,
                    TotalProjectEstimatedPrice = Math.Round(linearM * settings.MeterRateAcrylic, 0),
                    PriceDifferenceFromBaseline = 0m,
                    TierBadge = "الأكثر طلباً (الأساس)"
                },
                new MaterialVarianceOption
                {
                    MaterialName = "بولي لاك وسوبر مات حراري (Polylac / Super Matt)",
                    Description = "تقنية حرارية أوروبية مقاومة للبصمات (Anti-Fingerprint) والخدش بدرجة 3H ومقاومة للحرارة.",
                    PricePerMeter = settings.MeterRatePolylac,
                    TotalProjectEstimatedPrice = Math.Round(linearM * settings.MeterRatePolylac, 0),
                    PriceDifferenceFromBaseline = Math.Round((settings.MeterRatePolylac - baselineRate) * linearM, 0),
                    TierBadge = "فاخر وعصري"
                },
                new MaterialVarianceOption
                {
                    MaterialName = "زجاج سموكي عاكس بإطار بروفايل ألمنيوم وإضاءة LED",
                    Description = "درف زجاج مقسى (Tempered Glass) محاطة بإطار ألمنيوم أسود أو ذهبي مطفي مع إضاءة داخلية لكل رف.",
                    PricePerMeter = settings.MeterRateAlumGlass,
                    TotalProjectEstimatedPrice = Math.Round(linearM * settings.MeterRateAlumGlass, 0),
                    PriceDifferenceFromBaseline = Math.Round((settings.MeterRateAlumGlass - baselineRate) * linearM, 0),
                    TierBadge = "VIP فندقي راقٍ"
                }
            };

            // ========================================================
            //  2. مصفوفة مقارنة الإكسسوارات والميكانيزم (ديناميكي من الإعدادات)
            // ========================================================
            summary.HardwareVariances = new List<MechanismVarianceOption>
            {
                new MechanismVarianceOption
                {
                    MechanismName = "مفصلات عادية (Standard Non-Hydraulic)",
                    AppliedTo = "كافة الدرف العادية",
                    UnitCost = Math.Round(settings.PriceStandardHinges * 0.60m, 0),
                    SellingPrice = settings.PriceStandardHinges,
                    Advantage = "تكلفة اقتصادية منخفضة، لا تدعم الإغلاق الهادئ."
                },
                new MechanismVarianceOption
                {
                    MechanismName = "مفصلات بلوم النمساوية هيدروليك (Blum Soft-Close)",
                    AppliedTo = "الدرف والمطابخ الحديثة",
                    UnitCost = Math.Round(settings.PriceBlumSoftClose * 0.65m, 0),
                    SellingPrice = settings.PriceBlumSoftClose,
                    Advantage = "إغلاق سلس صامت مدى الحياة مع ضمان 100,000 فتحة وإغلاق."
                },
                new MechanismVarianceOption
                {
                    MechanismName = "رافعة أبواب علوية مزدوجة (Blum Aventos HF/HK)",
                    AppliedTo = "العلب العلوية وخزانة المطبقية",
                    UnitCost = Math.Round(settings.PriceBlumAventos * 0.70m, 0),
                    SellingPrice = settings.PriceBlumAventos,
                    Advantage = "تفتح الدرف للأعلى بزاوية حرة تمنع اصطدام الرأس وتتحمل أوزان الدرف الثقيلة."
                },
                new MechanismVarianceOption
                {
                    MechanismName = "سلة زاوية ذكية (Magic Corner / LeMans Tray)",
                    AppliedTo = "علب الزوايا العمياء (Blind Corner)",
                    UnitCost = Math.Round(settings.PriceMagicCorner * 0.70m, 0),
                    SellingPrice = settings.PriceMagicCorner,
                    Advantage = "استغلال 100% من عمق الزاوية الميتة مع إخراج الرفوف كاملة للخارج بسلاسة."
                },
                new MechanismVarianceOption
                {
                    MechanismName = "منظم ساعات ومجوهرات زجاجي مبطن مخمل",
                    AppliedTo = "أدراج حجرات الملابس والجزيرة",
                    UnitCost = Math.Round(settings.PriceJewelryOrganizer * 0.65m, 0),
                    SellingPrice = settings.PriceJewelryOrganizer,
                    Advantage = "تقسيمات داخلية مخملية فخمة مع واجهة زجاجية لعرض الساعات والخواتم."
                }
            };

            // ========================================================
            //  3. مصفوفة مقارنة أسطح العمل (الرخام والكوارتز)
            // ========================================================
            summary.CountertopVariances = new List<MaterialVarianceOption>
            {
                new MaterialVarianceOption
                {
                    MaterialName = "رخام صناعي أكريليك (Solid Surface)",
                    Description = "سطح متصل بدون لحامات ظاهرة، غير مسامي وسهل التلميع والإصلاح.",
                    PricePerMeter = settings.PriceCountertopArtificial,
                    TotalProjectEstimatedPrice = Math.Round(linearM * settings.PriceCountertopArtificial, 0),
                    PriceDifferenceFromBaseline = 0m,
                    TierBadge = "عملي ومتصل"
                },
                new MaterialVarianceOption
                {
                    MaterialName = "كوارتز ألماني / تركي (Quartz 93% Natural)",
                    Description = "مقاوم فائق للخدش والبقع والسكاكين، صلابة عالية ببريق بلوري فاخر.",
                    PricePerMeter = settings.PriceCountertopQuartz,
                    TotalProjectEstimatedPrice = Math.Round(linearM * settings.PriceCountertopQuartz, 0),
                    PriceDifferenceFromBaseline = Math.Round((settings.PriceCountertopQuartz - settings.PriceCountertopArtificial) * linearM, 0),
                    TierBadge = "الأعلى تحملاً وفخامة"
                },
                new MaterialVarianceOption
                {
                    MaterialName = "بورسلان مضغوط (Dekton / Compact Porcelain)",
                    Description = "مقاوم للحرارة المباشرة (النار والقدور الساخنة) والخدش والبهتان فوق البنفسجي.",
                    PricePerMeter = settings.PriceCountertopDekton,
                    TotalProjectEstimatedPrice = Math.Round(linearM * settings.PriceCountertopDekton, 0),
                    PriceDifferenceFromBaseline = Math.Round((settings.PriceCountertopDekton - settings.PriceCountertopArtificial) * linearM, 0),
                    TierBadge = "أعلى معايير المقاومة"
                },
                new MaterialVarianceOption
                {
                    MaterialName = "جرانيت طبيعي (جلاكسي / دبل بلاك إفريقي)",
                    Description = "حجر طبيعي صلب بلمعان أسود ملكي حبيبي مقاوم للصدمات.",
                    PricePerMeter = settings.PriceCountertopGranite,
                    TotalProjectEstimatedPrice = Math.Round(linearM * settings.PriceCountertopGranite, 0),
                    PriceDifferenceFromBaseline = Math.Round((settings.PriceCountertopGranite - settings.PriceCountertopArtificial) * linearM, 0),
                    TierBadge = "حجر طبيعي كلاسيكي"
                }
            };

            return summary;
        }

        // ============================================================
        //  توليد باقة علب افتراضية متكاملة بنقرة زر (Smart Generator)
        // ============================================================
        public static List<CabinetUnit> GenerateDefaultTemplateBoxes(int kitchenRequestId, CarpentryCategory category, PricingSetting settings = null)
        {
            settings ??= new PricingSetting();
            var list = new List<CabinetUnit>();

            if (category == CarpentryCategory.DressingRoom)
            {
                list.Add(new CabinetUnit
                {
                    KitchenRequestId = kitchenRequestId,
                    BoxCode = "DR-LONG-100",
                    Name = "باكية تعليق فساتين وعبايات طويلة",
                    Category = CabinetUnitCategory.DressingLongHang,
                    WidthCm = 100, HeightCm = 220, DepthCm = 60,
                    Carcass = CarcassMaterial.WhiteMelamineMdf,
                    DoorType = FrontDoorType.AluminiumFrameGlass,
                    Mechanism = MechanismType.BlumSoftCloseHinges,
                    HasLedLighting = true, HasGolaProfile = false
                });

                list.Add(new CabinetUnit
                {
                    KitchenRequestId = kitchenRequestId,
                    BoxCode = "DR-SHORT-90",
                    Name = "باكية تعليق مزدوج للقمصان والبدل",
                    Category = CabinetUnitCategory.DressingShortHang,
                    WidthCm = 90, HeightCm = 220, DepthCm = 60,
                    Carcass = CarcassMaterial.WhiteMelamineMdf,
                    DoorType = FrontDoorType.AluminiumFrameGlass,
                    Mechanism = MechanismType.BlumSoftCloseHinges,
                    HasLedLighting = true, HasGolaProfile = false
                });

                list.Add(new CabinetUnit
                {
                    KitchenRequestId = kitchenRequestId,
                    BoxCode = "DR-DRAWERS-80",
                    Name = "باكية أدراج مع منظم ساعات ومجوهرات زجاجي",
                    Category = CabinetUnitCategory.DressingDrawersAndJewelry,
                    WidthCm = 80, HeightCm = 220, DepthCm = 60,
                    Carcass = CarcassMaterial.WhiteMelamineMdf,
                    DoorType = FrontDoorType.AluminiumFrameGlass,
                    Mechanism = MechanismType.VelvetJewelryOrganizer,
                    HasLedLighting = true, HasGolaProfile = false
                });

                list.Add(new CabinetUnit
                {
                    KitchenRequestId = kitchenRequestId,
                    BoxCode = "DR-SHOES-80",
                    Name = "باكية أحذية مائلة وأرفف حقائب",
                    Category = CabinetUnitCategory.DressingShelvesAndShoes,
                    WidthCm = 80, HeightCm = 220, DepthCm = 45,
                    Carcass = CarcassMaterial.WhiteMelamineMdf,
                    DoorType = FrontDoorType.OpenWalkInNoDoors,
                    Mechanism = MechanismType.StandardHinges,
                    HasLedLighting = true, HasGolaProfile = false
                });

                list.Add(new CabinetUnit
                {
                    KitchenRequestId = kitchenRequestId,
                    BoxCode = "DR-ISLAND-120",
                    Name = "جزيرة دريسنج روم وسطية مع سطح زجاج وساعات",
                    Category = CabinetUnitCategory.DressingIsland,
                    WidthCm = 120, HeightCm = 90, DepthCm = 80,
                    Carcass = CarcassMaterial.WhiteMelamineMdf,
                    DoorType = FrontDoorType.PolylacSuperMatt,
                    Mechanism = MechanismType.TandemBoxDrawers,
                    HasLedLighting = true, HasGolaProfile = true
                });
            }
            else
            {
                list.Add(new CabinetUnit
                {
                    KitchenRequestId = kitchenRequestId,
                    BoxCode = "B-SINK-90",
                    Name = "علبة حوض معزولة مقاومة للماء",
                    Category = CabinetUnitCategory.BaseCabinet,
                    WidthCm = 90, HeightCm = 85, DepthCm = 60,
                    Carcass = CarcassMaterial.MarinePlywoodUnderSink,
                    DoorType = FrontDoorType.HighGlossAcrylic,
                    Mechanism = MechanismType.BlumSoftCloseHinges,
                    HasLedLighting = false, HasGolaProfile = true
                });

                list.Add(new CabinetUnit
                {
                    KitchenRequestId = kitchenRequestId,
                    BoxCode = "B-DRAW-80",
                    Name = "علبة أدراج 3 مستويات تاندم بوكس هيدروليك",
                    Category = CabinetUnitCategory.BaseCabinet,
                    WidthCm = 80, HeightCm = 85, DepthCm = 60,
                    Carcass = CarcassMaterial.MoistureResistantGreenHmr,
                    DoorType = FrontDoorType.HighGlossAcrylic,
                    Mechanism = MechanismType.TandemBoxDrawers,
                    HasLedLighting = false, HasGolaProfile = true
                });

                list.Add(new CabinetUnit
                {
                    KitchenRequestId = kitchenRequestId,
                    BoxCode = "B-CORNER-90",
                    Name = "علبة زاوية عمياء مع سلة ذكية LeMans",
                    Category = CabinetUnitCategory.BaseCabinet,
                    WidthCm = 100, HeightCm = 85, DepthCm = 60,
                    Carcass = CarcassMaterial.MoistureResistantGreenHmr,
                    DoorType = FrontDoorType.HighGlossAcrylic,
                    Mechanism = MechanismType.MagicCornerOrLeMans,
                    HasLedLighting = false, HasGolaProfile = true
                });

                list.Add(new CabinetUnit
                {
                    KitchenRequestId = kitchenRequestId,
                    BoxCode = "B-SPICE-25",
                    Name = "علبة سحب بهارات وزيوت استانلس",
                    Category = CabinetUnitCategory.BaseCabinet,
                    WidthCm = 25, HeightCm = 85, DepthCm = 60,
                    Carcass = CarcassMaterial.MoistureResistantGreenHmr,
                    DoorType = FrontDoorType.HighGlossAcrylic,
                    Mechanism = MechanismType.SpiceRackPullOut,
                    HasLedLighting = false, HasGolaProfile = true
                });

                list.Add(new CabinetUnit
                {
                    KitchenRequestId = kitchenRequestId,
                    BoxCode = "W-DISH-90",
                    Name = "علبة مطبقية ومصفاة صحون مع رافعة Aventos",
                    Category = CabinetUnitCategory.WallCabinet,
                    WidthCm = 90, HeightCm = 75, DepthCm = 35,
                    Carcass = CarcassMaterial.MoistureResistantGreenHmr,
                    DoorType = FrontDoorType.HighGlossAcrylic,
                    Mechanism = MechanismType.BlumAventosDoubleLift,
                    HasLedLighting = true, HasGolaProfile = false
                });

                list.Add(new CabinetUnit
                {
                    KitchenRequestId = kitchenRequestId,
                    BoxCode = "W-HOOD-90",
                    Name = "علبة شفاط مدمج علوية",
                    Category = CabinetUnitCategory.WallCabinet,
                    WidthCm = 90, HeightCm = 75, DepthCm = 35,
                    Carcass = CarcassMaterial.WhiteMelamineMdf,
                    DoorType = FrontDoorType.HighGlossAcrylic,
                    Mechanism = MechanismType.BlumSoftCloseHinges,
                    HasLedLighting = false, HasGolaProfile = false
                });

                list.Add(new CabinetUnit
                {
                    KitchenRequestId = kitchenRequestId,
                    BoxCode = "T-OVEN-60",
                    Name = "برج طولي لفرن ومايكروويف مدمج",
                    Category = CabinetUnitCategory.TallCabinet,
                    WidthCm = 60, HeightCm = 220, DepthCm = 60,
                    Carcass = CarcassMaterial.MoistureResistantGreenHmr,
                    DoorType = FrontDoorType.HighGlossAcrylic,
                    Mechanism = MechanismType.BlumSoftCloseHinges,
                    HasLedLighting = false, HasGolaProfile = true
                });
            }

            foreach (var b in list)
            {
                var (cost, price) = CalculateBoxCostAndPrice(b, settings);
                b.ManufacturingCost = cost;
                b.SellingPrice = price;
            }

            return list;
        }
    }
}
