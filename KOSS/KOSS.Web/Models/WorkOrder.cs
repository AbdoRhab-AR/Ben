using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOSS.Web.Models
{
    // ============================================================
    //  حالة أمر التنفيذ والتشغيل
    // ============================================================
    public enum WorkOrderStatus
    {
        [Display(Name = "قيد التخطيط وإصدار الـ BOM")]
        Planning = 1,

        [Display(Name = "قيد التصنيع والقص بالمصنع")]
        Manufacturing = 2,

        [Display(Name = "فحص الجودة والمطابقة الفنية")]
        QualityInspection = 3,

        [Display(Name = "جاهز للنقل والتركيب")]
        ReadyForInstallation = 4,

        [Display(Name = "قيد التركيب الميداني")]
        Installing = 5,

        [Display(Name = "معالجة ملاحظات ونواقص")]
        SnagResolution = 6,

        [Display(Name = "جاهز للتسليم النهائي")]
        ReadyForHandover = 7,

        [Display(Name = "مكتمل نهائياً")]
        Completed = 8,

        [Display(Name = "ملغى")]
        Cancelled = 9
    }

    // ============================================================
    //  أمر التنفيذ والتشغيل المركزي (WorkOrder)
    // ============================================================
    public class WorkOrder
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "طلب المطبخ")]
        public int KitchenRequestId { get; set; }

        [Display(Name = "العقد المرتبط")]
        public int? ContractId { get; set; }

        [Display(Name = "رقم أمر التشغيل")]
        [Required, StringLength(50)]
        public string OrderNumber { get; set; }

        [Display(Name = "مدير الإنتاج المسؤول")]
        public int? ProductionManagerId { get; set; }

        [Display(Name = "الموعد المستهدف لانتهاء التصنيع")]
        public DateTime? ExpectedEndDate { get; set; }

        [Display(Name = "تاريخ الانتهاء الفعلي من المصنع")]
        public DateTime? ActualEndDate { get; set; }

        [Display(Name = "حالة أمر التنفيذ")]
        public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Planning;

        [Display(Name = "ملاحظات وتوجيهات التصنيع")]
        [StringLength(1000)]
        public string ManufacturingNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }

        // العلاقات
        public virtual KitchenRequest KitchenRequest { get; set; }
        public virtual Contract Contract { get; set; }
        public virtual StaffMember ProductionManager { get; set; }

        public virtual ICollection<MaterialRequirement> MaterialRequirements { get; set; } = new List<MaterialRequirement>();
        public virtual ICollection<ManufacturingTask> Tasks { get; set; } = new List<ManufacturingTask>();
        public virtual ICollection<QualityCheck> QualityChecks { get; set; } = new List<QualityCheck>();
        public virtual ICollection<StockIssue> StockIssues { get; set; } = new List<StockIssue>();
        public virtual ICollection<InstallationOrder> InstallationOrders { get; set; } = new List<InstallationOrder>();
        public virtual ICollection<HandoverDocument> HandoverDocuments { get; set; } = new List<HandoverDocument>();
    }

    // ============================================================
    //  بند قائمة المواد المطلوب تجهيزها (MaterialRequirement / BOM)
    // ============================================================
    public class MaterialRequirement
    {
        public int Id { get; set; }

        [Required]
        public int WorkOrderId { get; set; }

        [Display(Name = "اسم الخامة / الصنف")]
        [Required, StringLength(200)]
        public string ItemName { get; set; }

        [Display(Name = "الفئة")]
        [Required, StringLength(100)]
        public string Category { get; set; }

        [Display(Name = "الكمية المطلوبة بالمطبخ")]
        public decimal QuantityRequired { get; set; }

        [Display(Name = "الوحدة القياسية")]
        [Required, StringLength(30)]
        public string Unit { get; set; } = "لوح";

        [Display(Name = "التكلفة التقديرية للوحدة (د.ل)")]
        public decimal EstimatedUnitCost { get; set; } = 0;

        [Display(Name = "إجمالي التكلفة التقديرية (د.ل)")]
        public decimal TotalEstimatedCost => QuantityRequired * EstimatedUnitCost;

        [Display(Name = "الكمية المتوفرة بالمخزن")]
        public decimal QuantityInStock { get; set; } = 0;

        [Display(Name = "الكمية التي تم صرفها للمشروع فعلياً")]
        public decimal QuantityIssued { get; set; } = 0;

        // العلاقة
        public virtual WorkOrder WorkOrder { get; set; }
    }
}
