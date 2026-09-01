using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOSS.Web.Models
{
    // ============================================================
    //  حالة أمر التنفيذ
    // ============================================================
    public enum WorkOrderStatus
    {
        [Display(Name = "قيد التخطيط")]
        Planning = 1,

        [Display(Name = "قيد التصنيع بالمصنع")]
        Manufacturing = 2,

        [Display(Name = "تم التصنيع - فحص الجودة")]
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

        [Display(Name = "رقم أمر التنفيذ")]
        [StringLength(50)]
        [Index("IX_WorkOrder_Number", IsUnique = true)]
        public string OrderNumber { get; set; }

        [Display(Name = "الأولوية")]
        public PriorityLevel Priority { get; set; } = PriorityLevel.Normal;

        [Display(Name = "مسؤول الإنتاج والتنفيذ")]
        public int? ProductionManagerId { get; set; }

        [Display(Name = "تاريخ البدء المخطط")]
        public DateTime? PlannedStartDate { get; set; }

        [Display(Name = "تاريخ الانتهاء المتوقع")]
        public DateTime? ExpectedEndDate { get; set; }

        [Display(Name = "تاريخ الانتهاء الفعلي")]
        public DateTime? ActualEndDate { get; set; }

        [Display(Name = "حالة أمر التنفيذ")]
        public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Planning;

        [Display(Name = "ملاحظات وتعليمات الإنتاج")]
        [StringLength(1000)]
        public string Notes { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "أُنشئ بواسطة")]
        public string CreatedBy { get; set; }

        // العلاقات
        public virtual KitchenRequest KitchenRequest { get; set; }
        public virtual Contract Contract { get; set; }
        public virtual StaffMember ProductionManager { get; set; }

        public virtual ICollection<MaterialRequirement> MaterialRequirements { get; set; } = new List<MaterialRequirement>();
        public virtual ICollection<StockIssue> StockIssues { get; set; } = new List<StockIssue>();
        public virtual ICollection<ManufacturingTask> Tasks { get; set; } = new List<ManufacturingTask>();
        public virtual ICollection<QualityCheck> QualityChecks { get; set; } = new List<QualityCheck>();
        public virtual ICollection<InstallationOrder> InstallationOrders { get; set; } = new List<InstallationOrder>();
        public virtual ICollection<HandoverDocument> HandoverDocuments { get; set; } = new List<HandoverDocument>();
    }

    // ============================================================
    //  قائمة المواد المطلوبة لأمر التنفيذ (MaterialRequirement - BOM)
    // ============================================================
    public class MaterialRequirement
    {
        public int Id { get; set; }

        [Required]
        public int WorkOrderId { get; set; }

        [Display(Name = "كود الصنف / المادة")]
        [StringLength(50)]
        public string ItemCode { get; set; }

        [Display(Name = "اسم المادة / الصنف")]
        [Required, StringLength(200)]
        public string ItemName { get; set; }

        [Display(Name = "فئة المادة")]
        [StringLength(100)]
        public string Category { get; set; }

        [Display(Name = "الوحدة")]
        [StringLength(30)]
        public string Unit { get; set; } = "قطعة";

        [Display(Name = "الكمية الإجمالية المطلوبة")]
        public decimal QuantityRequired { get; set; }

        [Display(Name = "الكمية المحجوزة من المخزن")]
        public decimal QuantityReserved { get; set; } = 0;

        [Display(Name = "الكمية المصروفة فعلياً للمشروع")]
        public decimal QuantityIssued { get; set; } = 0;

        [Display(Name = "الكمية الناقصة المطلوب شراؤها")]
        public decimal QuantityToPurchase => Math.Max(0, QuantityRequired - (QuantityReserved + QuantityIssued));

        [Display(Name = "تكلفة الوحدة التقديرية (د.ل)")]
        public decimal EstimatedUnitCost { get; set; } = 0;

        [Display(Name = "إجمالي التكلفة المقدرة (د.ل)")]
        public decimal TotalEstimatedCost => QuantityRequired * EstimatedUnitCost;

        [Display(Name = "هل تم اكتمال صرف البند؟")]
        public bool IsFullyIssued => QuantityIssued >= QuantityRequired;

        // العلاقة
        public virtual WorkOrder WorkOrder { get; set; }
    }
}
