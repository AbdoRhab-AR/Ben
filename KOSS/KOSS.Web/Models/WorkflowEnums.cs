using System;
using System.ComponentModel.DataAnnotations;

namespace KOSS.Web.Models
{
    // ============================================================
    //  1. حالات طلب المطبخ / المشروع الرئيسية (24 حالة صارمة)
    // ============================================================
    public enum KitchenRequestStatus
    {
        [Display(Name = "1. استفسار جديد")]
        NewInquiry = 1,

        [Display(Name = "2. مؤهل للمتابعة")]
        Qualified = 2,

        [Display(Name = "3. طلب مفتوح")]
        RequestOpened = 3,

        [Display(Name = "4. بانتظار المعاينة")]
        AwaitingSiteVisit = 4,

        [Display(Name = "5. تمت المعاينة")]
        SiteVisitCompleted = 5,

        [Display(Name = "6. قيد التصميم")]
        InDesign = 6,

        [Display(Name = "7. بانتظار اعتماد التصميم")]
        AwaitingDesignApproval = 7,

        [Display(Name = "8. قيد التسعير")]
        InPricing = 8,

        [Display(Name = "9. عرض مرسل")]
        QuotationSent = 9,

        [Display(Name = "10. تفاوض أو تعديل")]
        NegotiationOrRevision = 10,

        [Display(Name = "11. عرض مقبول")]
        QuotationAccepted = 11,

        [Display(Name = "12. بانتظار العقد والعربون")]
        AwaitingContractAndDeposit = 12,

        [Display(Name = "13. عقد نشط")]
        ContractActive = 13,

        [Display(Name = "14. قيد التخطيط")]
        InPlanning = 14,

        [Display(Name = "15. قيد التصنيع")]
        InManufacturing = 15,

        [Display(Name = "16. جاهز للتركيب")]
        ReadyForInstallation = 16,

        [Display(Name = "17. مجدول للتركيب")]
        InstallationScheduled = 17,

        [Display(Name = "18. قيد التركيب")]
        InInstallation = 18,

        [Display(Name = "19. بانتظار معالجة ملاحظات")]
        AwaitingSnagResolution = 19,

        [Display(Name = "20. جاهز للتسليم")]
        ReadyForHandover = 20,

        [Display(Name = "21. تم التسليم")]
        HandoverCompleted = 21,

        [Display(Name = "22. بانتظار الرصيد")]
        AwaitingFinalBalance = 22,

        [Display(Name = "23. مغلق ومراجع")]
        Closed = 23,

        [Display(Name = "24. ملغى أو مرفوض")]
        CancelledOrRejected = 24
    }

    // ============================================================
    //  2. أنواع المطابخ والتخطيط
    // ============================================================
    public enum KitchenLayoutType
    {
        [Display(Name = "خطي مستقيم (Straight)")]
        Straight = 1,

        [Display(Name = "حرف L")]
        LShape = 2,

        [Display(Name = "حرف U")]
        UShape = 3,

        [Display(Name = "مطبخ مع جزيرة (Island)")]
        Island = 4,

        [Display(Name = "مطبخ موازٍ (Galley)")]
        Galley = 5
    }

    // ============================================================
    //  3. نوع المشروع
    // ============================================================
    public enum ProjectType
    {
        [Display(Name = "فيلا سكنية")]
        Villa = 1,

        [Display(Name = "شقة سكنية")]
        Apartment = 2,

        [Display(Name = "مكتب / شركة")]
        Office = 3,

        [Display(Name = "مشروع تجاري")]
        Commercial = 4
    }

    // ============================================================
    //  4. حالات المعاينة
    // ============================================================
    public enum SiteVisitStatus
    {
        [Display(Name = "مجدولة")]
        Scheduled = 1,

        [Display(Name = "قيد التنفيذ")]
        InProgress = 2,

        [Display(Name = "تمت الزيارة")]
        Visited = 3,

        [Display(Name = "بانتظار المراجعة")]
        AwaitingReview = 4,

        [Display(Name = "معتمدة")]
        Approved = 5,

        [Display(Name = "تحتاج إعادة زيارة")]
        NeedsRevisit = 6
    }

    // ============================================================
    //  5. حالات إصدار التصميم
    // ============================================================
    public enum DesignVersionStatus
    {
        [Display(Name = "مسودة")]
        Draft = 1,

        [Display(Name = "قيد التنفيذ")]
        InProgress = 2,

        [Display(Name = "مراجعة داخلية")]
        InternalReview = 3,

        [Display(Name = "مرسل للعميل")]
        SentToCustomer = 4,

        [Display(Name = "تعديل مطلوب")]
        RevisionRequired = 5,

        [Display(Name = "معتمد من العميل")]
        ApprovedByCustomer = 6
    }

    // ============================================================
    //  6. حالات عرض السعر
    // ============================================================
    public enum QuotationStatus
    {
        [Display(Name = "مسودة")]
        Draft = 1,

        [Display(Name = "بانتظار الاعتماد الداخلي")]
        AwaitingInternalApproval = 2,

        [Display(Name = "معتمد داخلياً")]
        Approved = 3,

        [Display(Name = "مرسل للعميل")]
        SentToCustomer = 4,

        [Display(Name = "تعديل مطلوب")]
        RevisionRequested = 5,

        [Display(Name = "مقبول")]
        Accepted = 6,

        [Display(Name = "مرفوض")]
        Rejected = 7
    }

    // ============================================================
    //  7. أولويات العمل
    // ============================================================
    public enum PriorityLevel
    {
        [Display(Name = "عادي")]
        Normal = 1,

        [Display(Name = "مهم")]
        Important = 2,

        [Display(Name = "عاجل جداً")]
        Urgent = 3
    }
}
