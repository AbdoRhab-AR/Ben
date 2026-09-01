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
                KitchenRequestStatus.Closed => "23. مغلق ومكتمل نهائياً",
                KitchenRequestStatus.CancelledOrRejected => "24. ملغى أو مرفوض",
                KitchenRequestStatus.Suspended => "موقوف مؤقتاً",
                _ => status.ToString()
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
                _ => layout.ToString()
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
