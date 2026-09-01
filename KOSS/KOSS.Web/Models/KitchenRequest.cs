using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace KOSS.Web.Models
{
    // ============================================================
    //  طلب المطبخ / المشروع المركزي (KitchenRequest / Project Core)
    // ============================================================
    public class KitchenRequest
    {
        public int Id { get; set; }

        [Display(Name = "رقم الطلب الرسمي")]
        [StringLength(50)]
        [Index("IX_KitchenRequest_Number", IsUnique = true)]
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

        [Display(Name = "سبب الإلغاء / الرفض (إن وجد)")]
        [StringLength(300)]
        public string CancellationReason { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "تاريخ آخر تحديث")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Display(Name = "أُنشئ بواسطة")]
        public string CreatedBy { get; set; }

        // ============================================================
        //  خصائص محسوبة للربط السريع
        // ============================================================
        [NotMapped]
        public Contract ActiveContract => Contracts != null ? Contracts.FirstOrDefault(c => c.Status == ContractStatus.Active || c.Status == ContractStatus.Completed) : null;

        [NotMapped]
        public DesignVersion ApprovedDesign => DesignVersions != null ? DesignVersions.FirstOrDefault(d => d.Status == DesignVersionStatus.ApprovedByCustomer) : null;

        [NotMapped]
        public Quotation AcceptedQuotation => Quotations != null ? Quotations.FirstOrDefault(q => q.Status == QuotationStatus.Accepted) : null;

        [NotMapped]
        public SiteVisit ApprovedSiteVisit => SiteVisits != null ? SiteVisits.FirstOrDefault(s => s.Status == SiteVisitStatus.Approved) : null;

        [NotMapped]
        public WorkOrder CurrentWorkOrder => WorkOrders != null ? WorkOrders.FirstOrDefault(w => w.Status != WorkOrderStatus.Cancelled) : null;

        // ============================================================
        //  العلاقات المتفرعة عن طلب المطبخ
        // ============================================================
        public virtual Customer Customer { get; set; }
        public virtual StaffMember AssignedSalesStaff { get; set; }

        public virtual ICollection<RequestStatusHistory> StatusHistories { get; set; } = new List<RequestStatusHistory>();
        public virtual ICollection<SiteVisit> SiteVisits { get; set; } = new List<SiteVisit>();
        public virtual ICollection<DesignVersion> DesignVersions { get; set; } = new List<DesignVersion>();
        public virtual ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();
        public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
        public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
        public virtual ICollection<ProjectExpense> Expenses { get; set; } = new List<ProjectExpense>();
    }

    // ============================================================
    //  سجل الحالات التاريخي للطلب (RequestStatusHistory)
    // ============================================================
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

        // العلاقة
        public virtual KitchenRequest KitchenRequest { get; set; }
    }
}
