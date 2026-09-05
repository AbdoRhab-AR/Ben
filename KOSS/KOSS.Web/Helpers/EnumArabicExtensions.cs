using System;
using KOSS.Web.Models;

namespace KOSS.Web.Helpers
{
    public static class EnumArabicExtensions
    {
        public static string ToArabic(this KitchenRequestStatus status)
        {
            return status switch
            {
                KitchenRequestStatus.NewInquiry => "1. استفسار وفرصة بيع",
                KitchenRequestStatus.Qualified => "2. مؤهل للمتابعة",
                KitchenRequestStatus.RequestOpened => "3. تم فتح طلب المطبخ",
                KitchenRequestStatus.AwaitingSiteVisit => "4. بانتظار المعاينة الميدانية",
                KitchenRequestStatus.SiteVisitCompleted => "5. تمت المعاينة والقياسات",
                KitchenRequestStatus.InDesign => "6. قيد التصميم 3D",
                KitchenRequestStatus.AwaitingDesignApproval => "7. بانتظار اعتماد التصميم",
                KitchenRequestStatus.InPricing => "8. قيد التسعير وحساب التكلفة",
                KitchenRequestStatus.QuotationSent => "9. تم إرسال عرض السعر للعميل",
                KitchenRequestStatus.NegotiationOrRevision => "10. تفاوض أو تعديل على العرض",
                KitchenRequestStatus.QuotationAccepted => "11. تم قبول عرض السعر",
                KitchenRequestStatus.AwaitingContractAndDeposit => "12. بانتظار العقد والعربون",
                KitchenRequestStatus.ContractActive => "13. عقد نشط - بدء التشغيل",
                KitchenRequestStatus.InPlanning => "14. تخطيط الخامات وإصدار الـ BOM",
                KitchenRequestStatus.InManufacturing => "15. قيد التصنيع والقص بالمصنع",
                KitchenRequestStatus.ReadyForInstallation => "16. جاهز للنقل والتركيب",
                KitchenRequestStatus.InstallationScheduled => "17. جدولة موعد التركيب بالموقع",
                KitchenRequestStatus.InInstallation => "18. قيد التركيب الميداني",
                KitchenRequestStatus.AwaitingSnagResolution => "19. معالجة ملاحظات ونواقص",
                KitchenRequestStatus.ReadyForHandover => "20. جاهز للتسليم النهائي",
                KitchenRequestStatus.HandoverCompleted => "21. تم التسليم وتوقيع المحضر",
                KitchenRequestStatus.AwaitingFinalBalance => "22. بانتظار سداد المخالصة النهائية",
                KitchenRequestStatus.Closed => "23. مشروع مغلق ومكتمل نهائياً",
                KitchenRequestStatus.CancelledOrRejected => "24. ملغى أو مرفوض",
                KitchenRequestStatus.Suspended => "موقوف مؤقتاً",
                _ => status.ToString()
            };
        }

        public static string ToArabic(this CarpentryCategory category)
        {
            return category switch
            {
                CarpentryCategory.Kitchen => "مطبخ حديث (Modern Kitchen)",
                CarpentryCategory.DressingRoom => "حجرة ملابس (Dressing Room)",
                CarpentryCategory.Wardrobe => "دولاب حائط مدمج (Built-in Wardrobe)",
                CarpentryCategory.Combined => "مطبخ ودواليب متعددة (Combined)",
                _ => category.ToString()
            };
        }

        public static string ToArabic(this ProjectType type)
        {
            return type switch
            {
                ProjectType.Villa => "فيلا سكنية",
                ProjectType.Apartment => "شقة سكنية",
                ProjectType.Commercial => "مشروع تجاري / معرض",
                ProjectType.Chalet => "شاليه / استراحة",
                _ => type.ToString()
            };
        }

        public static string ToArabic(this KitchenLayoutType layout)
        {
            return layout switch
            {
                KitchenLayoutType.Straight => "خطي مستقيم (Straight)",
                KitchenLayoutType.LShaped => "حرف L",
                KitchenLayoutType.UShaped => "حرف U",
                KitchenLayoutType.Parallel => "مطبخ متوازي (Galley)",
                KitchenLayoutType.Island => "مطبخ مع جزيرة وسطية (Island)",
                KitchenLayoutType.OpenWalkIn => "دريسنج روم مفتوح بدون درف (Walk-in)",
                KitchenLayoutType.SlidingWardrobe => "دولاب درف سحاب (Sliding)",
                KitchenLayoutType.HingedAluminiumGlass => "دواليب درف زجاج وإطار ألمنيوم",
                KitchenLayoutType.IslandDressing => "دريسنج روم مع جزيرة ساعات ومجوهرات",
                _ => layout.ToString()
            };
        }

        public static string ToArabic(this CabinetUnitCategory category)
        {
            return category switch
            {
                CabinetUnitCategory.BaseCabinet => "علبة سفلية (Base)",
                CabinetUnitCategory.WallCabinet => "علبة علوية (Wall)",
                CabinetUnitCategory.TallCabinet => "برج طولي (Tall Tower)",
                CabinetUnitCategory.IslandCabinet => "علبة جزيرة وسطية (Island)",
                CabinetUnitCategory.LoftCabinet => "علبة سقفية علوية (Loft)",
                CabinetUnitCategory.DressingLongHang => "باكية تعليق طويل (Long Hang)",
                CabinetUnitCategory.DressingShortHang => "باكية تعليق قصير (Short Hang)",
                CabinetUnitCategory.DressingDrawersAndJewelry => "باكية أدراج وساعات ومجوهرات",
                CabinetUnitCategory.DressingShelvesAndShoes => "باكية أرفف ملابس وأحذية",
                CabinetUnitCategory.DressingIsland => "جزيرة دريسنج روم وسطية",
                _ => category.ToString()
            };
        }

        public static string ToArabic(this CarcassMaterial mat)
        {
            return mat switch
            {
                CarcassMaterial.WhiteMelamineMdf => "MDF أبيض تركي مكسو ميلامين",
                CarcassMaterial.MoistureResistantGreenHmr => "MDF أخضر مقاوم للرطوبة (HMR)",
                CarcassMaterial.MarinePlywoodUnderSink => "خشب كونتر معزول مقاوم للماء (للحوض)",
                CarcassMaterial.Chipboard => "خشب حبيبي معالج اقتصادي (Chipboard)",
                _ => mat.ToString()
            };
        }

        public static string ToArabic(this FrontDoorType door)
        {
            return door switch
            {
                FrontDoorType.HighGlossAcrylic => "أكريليك عالي اللمعان (High Gloss)",
                FrontDoorType.PolylacSuperMatt => "بولي لاك وسوبر مات حراري (Polylac)",
                FrontDoorType.MelamineFormica => "ميلامين / فورميكا اقتصادي (HPL)",
                FrontDoorType.AluminiumFrameGlass => "زجاج سموكي عاكس بإطار بروفايل ألمنيوم",
                FrontDoorType.OpenWalkInNoDoors => "مفتوحة بدون درف (Open Concept)",
                _ => door.ToString()
            };
        }

        public static string ToArabic(this MechanismType mech)
        {
            return mech switch
            {
                MechanismType.StandardHinges => "مفصلات عادية",
                MechanismType.BlumSoftCloseHinges => "مفصلات بلوم هيدروليك Soft-Close",
                MechanismType.BlumAventosDoubleLift => "رافعة هيدروليكية مزدوجة (Blum Aventos)",
                MechanismType.MagicCornerOrLeMans => "سلة زاوية ذكية (Magic Corner / LeMans)",
                MechanismType.TandemBoxDrawers => "أدراج تاندم بوكس هيدروليك",
                MechanismType.VelvetJewelryOrganizer => "منظم ساعات ومجوهرات زجاجي مبطن مخمل",
                MechanismType.PullOutTrouserRack => "علاقة بناطيل سحب هيدروليكية",
                MechanismType.SpiceRackPullOut => "سلة سحب بهارات وزيوت",
                _ => mech.ToString()
            };
        }

        public static string ToArabic(this PricingMethod method)
        {
            return method switch
            {
                PricingMethod.RunningMeter => "بالمتر الطولي (Running Meter)",
                PricingMethod.SquareMeter => "بالمتر المربع (Square Meter)",
                PricingMethod.ModularBoxPricing => "تسعير تجميعي بالعلبة (Modular Box)",
                _ => method.ToString()
            };
        }

        public static string ToArabic(this QuotationStatus status)
        {
            return status switch
            {
                QuotationStatus.Draft => "مسودة تسعير",
                QuotationStatus.InternalApproved => "معتمد داخلياً",
                QuotationStatus.SentToCustomer => "مُرسل للعميل",
                QuotationStatus.Accepted => "مقبول من العميل",
                QuotationStatus.Rejected => "مرفوض",
                QuotationStatus.Revised => "مُعدّل بإصدار جديد",
                _ => status.ToString()
            };
        }

        public static string ToArabic(this ContractStatus status)
        {
            return status switch
            {
                ContractStatus.Draft => "مسودة عقد",
                ContractStatus.AwaitingDeposit => "بانتظار سداد العربون",
                ContractStatus.Active => "عقد سارٍ ونشط",
                ContractStatus.Suspended => "عقد موقوف",
                ContractStatus.Completed => "عقد مكتمل ومغلق",
                ContractStatus.Cancelled => "عقد ملغى",
                _ => status.ToString()
            };
        }

        public static string ToArabic(this WorkOrderStatus status)
        {
            return status switch
            {
                WorkOrderStatus.Planning => "قيد التخطيط وإصدار الـ BOM",
                WorkOrderStatus.Manufacturing => "قيد التصنيع والقص بالمصنع",
                WorkOrderStatus.QualityInspection => "فحص الجودة والمطابقة",
                WorkOrderStatus.ReadyForInstallation => "جاهز للنقل والتركيب",
                WorkOrderStatus.Installing => "قيد التركيب الميداني",
                WorkOrderStatus.SnagResolution => "معالجة النواقص",
                WorkOrderStatus.ReadyForHandover => "جاهز للتسليم",
                WorkOrderStatus.Completed => "مكتمل نهائياً",
                WorkOrderStatus.Cancelled => "ملغى",
                _ => status.ToString()
            };
        }

        public static string ToArabic(this DesignVersionStatus status)
        {
            return status switch
            {
                DesignVersionStatus.Draft => "مسودة تصميم",
                DesignVersionStatus.InternalReview => "مراجعة داخلية",
                DesignVersionStatus.SentToCustomer => "بانتظار رد العميل",
                DesignVersionStatus.ApprovedByCustomer => "معتمد من العميل",
                DesignVersionStatus.RejectedByCustomer => "مرفوض من العميل",
                _ => status.ToString()
            };
        }

        public static string ToArabic(this SiteVisitStatus status)
        {
            return status switch
            {
                SiteVisitStatus.Scheduled => "معاينة مجدولة",
                SiteVisitStatus.InProgress => "قيد التنفيذ",
                SiteVisitStatus.Completed => "تمت الزيارة الميدانية",
                SiteVisitStatus.AwaitingReview => "القياسات بانتظار الاعتماد",
                SiteVisitStatus.Approved => "قياسات معتمدة",
                SiteVisitStatus.Cancelled => "معاينة ملغاة",
                _ => status.ToString()
            };
        }

        public static string ToArabic(this PaymentMethod method)
        {
            return method switch
            {
                PaymentMethod.Cash => "نقداً (كاش)",
                PaymentMethod.BankTransfer => "تحويل مصرفي",
                PaymentMethod.Cheque => "صك مصدق",
                _ => method.ToString()
            };
        }
    }
}
