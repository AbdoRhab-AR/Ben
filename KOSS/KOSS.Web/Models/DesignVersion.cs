using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOSS.Web.Models
{
    // ============================================================
    //  إصدارات التصميم (DesignVersion) - دعم الإصدارات المتعددة والاعتماد
    // ============================================================
    public class DesignVersion
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "طلب المطبخ")]
        public int KitchenRequestId { get; set; }

        [Display(Name = "رقم الإصدار")]
        public int VersionNumber { get; set; } = 1;

        [Display(Name = "رمز الإصدار")]
        [StringLength(20)]
        public string VersionCode => $"V{VersionNumber}.0";

        [Display(Name = "المصمم المسؤول")]
        public int? DesignerId { get; set; }

        [Display(Name = "برنامج التصميم المستخدم")]
        [StringLength(100)]
        public string SoftwareUsed { get; set; } = "SketchUp";

        [Display(Name = "إجمالي الأمتار الطولية للخزائن")]
        public decimal? EstimatedLinearMeters { get; set; }

        [Display(Name = "رابط ملف المخطط ثلاثي الأبعاد 3D")]
        [StringLength(500)]
        public string DesignFilePath { get; set; }

        [Display(Name = "روابط الصور والرندرات")]
        [StringLength(1000)]
        public string RenderImagesPaths { get; set; }

        [Display(Name = "ملاحظات وشرح التصميم")]
        [StringLength(1000)]
        public string Notes { get; set; }

        [Display(Name = "حالة إصدار التصميم")]
        public DesignVersionStatus Status { get; set; } = DesignVersionStatus.Draft;

        [Display(Name = "هل الإصدار مقفل؟")]
        public bool IsLocked { get; set; } = false;

        [Display(Name = "تاريخ اعتماد العميل")]
        public DateTime? CustomerApprovedAt { get; set; }

        [Display(Name = "ملاحظات وملاحظات العميل")]
        [StringLength(500)]
        public string CustomerFeedback { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "أُنشئ بواسطة")]
        public string CreatedBy { get; set; }

        // العلاقات
        [ForeignKey("KitchenRequestId")]
        public virtual KitchenRequest KitchenRequest { get; set; }

        [ForeignKey("DesignerId")]
        public virtual StaffMember Designer { get; set; }
        public virtual ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();
    }
}
