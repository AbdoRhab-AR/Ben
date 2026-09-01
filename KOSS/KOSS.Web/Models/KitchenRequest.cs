using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace KOSS.Web.Models
{
    public enum ProjectType
    {
        [Display(Name = "فيلا سكنية")]
        Villa = 1,

        [Display(Name = "شقة سكنية")]
        Apartment = 2,

        [Display(Name = "مشروع تجاري / معرض")]
        Commercial = 3,

        [Display(Name = "شاليه / استراحة")]
        Chalet = 4
    }

    public enum KitchenLayoutType
    {
        [Display(Name = "خطي مستقيم (Straight)")]
        Straight = 1,

        [Display(Name = "حرف L (L-Shaped)")]
        LShaped = 2,

        [Display(Name = "حرف U (U-Shaped)")]
        UShaped = 3,

        [Display(Name = "متوازي (Galley / Parallel)")]
        Parallel = 4,

        [Display(Name = "مع جزيرة وسطية (Island)")]
        Island = 5
    }

    public enum KitchenRequestStatus
    {
        [Display(Name = "1. استفسار وفرصة بيع")]
        NewInquiry = 1,

        [Display(Name = "2. مؤهل للمتابعة")]
        Qualified = 2,

        [Display(Name = "3. طلب مطبخ مفتوح")]
        RequestOpened = 3,

        [Display(Name = "4. بانتظار المعاينة الميدانية")]
        AwaitingSiteVisit = 4,

        [Display(Name = "5. تمت المعاينة والقياسات")]
        SiteVisitCompleted = 5,

        [Display(Name = "6. قيد التصميم 3D")]
        InDesign = 6,

        [Display(Name = "7. بانتظار اعتماد التصميم")]
        AwaitingDesignApproval = 7,

        [Display(Name = "8. قيد التسعير وحساب التكلفة")]
        InPricing = 8,

        [Display(Name = "9. عرض السعر مُرسل للعميل")]
        QuotationSent = 9,

        [Display(Name = "10. تفاوض أو تعديل على العرض")]
        NegotiationOrRevision = 10,

        [Display(Name = "11. تم قبول عرض السعر")]
        QuotationAccepted = 11,

        [Display(Name = "12. بانتظار توقيع العقد وسداد العربون")]
        AwaitingContractAndDeposit = 12,

        [Display(Name = "13. عقد نشط - بدء التشغيل")]
        ContractActive = 13,

        [Display(Name = "14. تخطيط الخامات وإصدار الـ BOM")]
        InPlanning = 14,

        [Display(Name = "15. قيد التصنيع والقص بالمصنع")]
        InManufacturing = 15,

        [Display(Name = "16. جاهز للنقل والتركيب")]
        ReadyForInstallation = 16,

        [Display(Name = "17. جدولة موعد التركيب بالموقع")]
        InstallationScheduled = 17,

        [Display(Name = "18. قيد التركيب الميداني")]
        InInstallation = 18,

        [Display(Name = "19. بانتظار معالجة ملاحظات ونواقص")]
        AwaitingSnagResolution = 19,

        [Display(Name = "20. جاهز للتسليم النهائي")]
        ReadyForHandover = 20,

        [Display(Name = "21. تم التسليم وتوقيع المحضر")]
        HandoverCompleted = 21,

        [Display(Name = "22. بانتظار سداد المخالصة النهائية")]
        AwaitingFinalBalance = 22,

        [Display(Name = "23. مشروع مغلق ومكتمل نهائياً")]
        Closed = 23,

        [Display(Name = "24. ملغى أو مرفوض")]
        CancelledOrRejected = 24,

        [Display(Name = "موقوف مؤقتاً")]
        Suspended = 98
    }

    public class KitchenRequest
    {
        public int Id { get; set; }

        [Display(Name = "رقم الطلب الرسمي")]
        [StringLength(50)]
        public string RequestNumber { get; set; }

        [Required]
        [Display(Name = "العميل")]
        public int CustomerId { get; set; }

        [Display(Name = "عنوان وموقع المطبخ")]
        [Required(ErrorMessage = "موقع المطبخ مطلوب"), StringLength(250)]
        public string Location { get; set; }

        [Display(Name = "نوع المشروع")]
        public ProjectType ProjectType { get; set; } = ProjectType.Villa;

        [Display(Name = "تخطيط المطبخ")]
        public KitchenLayoutType LayoutType { get; set; } = KitchenLayoutType.Straight;

        [Display(Name = "موظف المبيعات المسؤول")]
        public int? AssignedSalesStaffId { get; set; }

        [Display(Name = "الموعد المتوقع للتسليم")]
        public DateTime? TargetDeliveryDate { get; set; }

        [Display(Name = "الحالة الراهنة")]
        public KitchenRequestStatus Status { get; set; } = KitchenRequestStatus.RequestOpened;

        [Display(Name = "الملاحظات والاحتياجات الأولية")]
        [StringLength(1000)]
        public string Notes { get; set; }

        [Display(Name = "سبب الإلغاء / الرفض")]
        [StringLength(300)]
        public string CancellationReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual StaffMember AssignedSalesStaff { get; set; }

        public virtual ICollection<RequestStatusHistory> StatusHistories { get; set; } = new List<RequestStatusHistory>();
        public virtual ICollection<SiteVisit> SiteVisits { get; set; } = new List<SiteVisit>();
        public virtual ICollection<DesignVersion> DesignVersions { get; set; } = new List<DesignVersion>();
        public virtual ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();
        public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
        public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
        public virtual ICollection<ProjectExpense> Expenses { get; set; } = new List<ProjectExpense>();

        [NotMapped]
        public Contract ActiveContract => Contracts?.FirstOrDefault(c => c.Status == ContractStatus.Active) ?? Contracts?.LastOrDefault();

        [NotMapped]
        public WorkOrder CurrentWorkOrder => WorkOrders?.FirstOrDefault(w => w.Status != WorkOrderStatus.Cancelled) ?? WorkOrders?.LastOrDefault();
    }

    public class RequestStatusHistory
    {
        public int Id { get; set; }

        [Required]
        public int KitchenRequestId { get; set; }

        [Display(Name = "الحالة السابقة")]
        public KitchenRequestStatus OldStatus { get; set; }

        [Display(Name = "الحالة الجديدة")]
        public KitchenRequestStatus NewStatus { get; set; }

        [Display(Name = "سبب الانتقال / الملاحظات")]
        [StringLength(500)]
        public string Notes { get; set; }

        [Display(Name = "تاريخ ووقت التغيير")]
        public DateTime ChangedAt { get; set; } = DateTime.Now;

        [Display(Name = "المستخدم الذي نفذ التغيير")]
        public string ChangedBy { get; set; }

        public virtual KitchenRequest KitchenRequest { get; set; }
    }
}
