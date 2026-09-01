using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOSS.Web.Models
{
    public enum DesignVersionStatus
    {
        [Display(Name = "مسودة تصميم")]
        Draft = 1,

        [Display(Name = "مراجعة داخلية")]
        InternalReview = 2,

        [Display(Name = "بانتظار رد واعتماد العميل")]
        SentToCustomer = 3,

        [Display(Name = "تعديل مطلوب")]
        RevisionRequired = 4,

        [Display(Name = "معتمد رسمياً من العميل")]
        ApprovedByCustomer = 5,

        [Display(Name = "مرفوض من العميل")]
        RejectedByCustomer = 6
    }

    public class DesignVersion
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "طلب المطبخ")]
        public int KitchenRequestId { get; set; }

        [Display(Name = "المصمم الداخلي المسؤول")]
        public int? DesignerId { get; set; }

        [Display(Name = "رقم الإصدار")]
        public int VersionNumber { get; set; } = 1;

        [Display(Name = "كود الإصدار (V1, V2, ...)")]
        [Required, StringLength(20)]
        public string VersionCode { get; set; } = "V1.0";

        [Display(Name = "البرنامج المستخدم في التصميم والريندر")]
        [StringLength(100)]
        public string SoftwareUsed { get; set; } = "2020 Design";

        [Display(Name = "إجمالي الأمتار الطولية المحسوبة بالمخطط")]
        public decimal EstimatedLinearMeters { get; set; } = 0;

        [Display(Name = "رابط ملفات التصميم والريندر 3D")]
        [StringLength(500)]
        public string RenderFilesPath { get; set; }

        [Display(Name = "ملاحظات ومواصفات التصميم")]
        [StringLength(1000)]
        public string Notes { get; set; }

        [Display(Name = "حالة التصميم")]
        public DesignVersionStatus Status { get; set; } = DesignVersionStatus.Draft;

        [Display(Name = "ملاحظات وتعديلات العميل")]
        [StringLength(500)]
        public string CustomerFeedback { get; set; }

        [Display(Name = "هل التصميم مقفل؟")]
        public bool IsLocked { get; set; } = false;

        [Display(Name = "تاريخ اعتماد العميل")]
        public DateTime? CustomerApprovedAt { get; set; }

        [Display(Name = "تاريخ الاعتماد")]
        public DateTime? ApprovedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }

        // العلاقات
        public virtual KitchenRequest KitchenRequest { get; set; }
        public virtual StaffMember Designer { get; set; }
        public virtual ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();
    }
}
