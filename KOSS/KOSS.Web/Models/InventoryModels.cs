using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOSS.Web.Models
{
    // ============================================================
    //  أنواع الحركات المخزنية
    // ============================================================
    public enum StockTransactionType
    {
        [Display(Name = "استلام مخزني (شراء / توريد)")]
        Receipt = 1,

        [Display(Name = "صرف لمشروع مطبخ")]
        IssueToProject = 2,

        [Display(Name = "صرف تشغيلي عام")]
        IssueGeneral = 3,

        [Display(Name = "مناقلة بين مستودعين")]
        Transfer = 4,

        [Display(Name = "مرتجع من مشروع للمخزن")]
        ReturnFromProject = 5,

        [Display(Name = "مرتجع لمورد")]
        ReturnToSupplier = 6,

        [Display(Name = "تسوية جردية (+ / -)")]
        InventoryAdjustment = 7
    }

    // ============================================================
    //  المستودع (Warehouse)
    // ============================================================
    public class Warehouse
    {
        public int Id { get; set; }

        [Display(Name = "كود المستودع")]
        [Required, StringLength(30)]
        public string Code { get; set; }

        [Display(Name = "اسم المستودع")]
        [Required, StringLength(150)]
        public string Name { get; set; }

        [Display(Name = "الموقع الجغرافي")]
        [StringLength(200)]
        public string Location { get; set; }

        [Display(Name = "أمين المستودع")]
        [StringLength(100)]
        public string KeeperName { get; set; }

        [Display(Name = "هل المستودع نشط؟")]
        public bool IsActive { get; set; } = true;

        // العلاقات
        public virtual ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();
        public virtual ICollection<StockIssue> StockIssues { get; set; } = new List<StockIssue>();
    }

    // ============================================================
    //  دليل الأصناف والمواد القياسي (ItemMaster)
    // ============================================================
    public class ItemMaster
    {
        public int Id { get; set; }

        [Display(Name = "كود الصنف المعياري")]
        [Required, StringLength(50)]
        public string ItemCode { get; set; }

        [Display(Name = "اسم المادة / الصنف")]
        [Required, StringLength(200)]
        public string Name { get; set; }

        [Display(Name = "الفئة الرئيسية")]
        [Required, StringLength(100)]
        public string Category { get; set; }

        [Display(Name = "الوحدة القياسية")]
        [Required, StringLength(30)]
        public string Unit { get; set; } = "قطعة";

        [Display(Name = "التكلفة القياسية (د.ل)")]
        public decimal StandardCost { get; set; } = 0;

        [Display(Name = "سعر البيع القياسي (د.ل)")]
        public decimal StandardSalePrice { get; set; } = 0;

        [Display(Name = "حد إعادة الطلب الأدنى")]
        public decimal ReorderLevel { get; set; } = 5;

        [Display(Name = "المواصفات الفنية")]
        [StringLength(500)]
        public string Specifications { get; set; }

        [Display(Name = "هل الصنف نشط؟")]
        public bool IsActive { get; set; } = true;

        // العلاقات
        public virtual ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();
    }

    // ============================================================
    //  رصيد الصنف بالمستودع (StockItem)
    // ============================================================
    public class StockItem
    {
        public int Id { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        [Required]
        public int ItemMasterId { get; set; }

        [Display(Name = "الرصيد الفعلي بالمخزن")]
        public decimal PhysicalQuantity { get; set; } = 0;

        [Display(Name = "الرصيد المحجوز للمشاريع")]
        public decimal ReservedQuantity { get; set; } = 0;

        [Display(Name = "الرصيد المتاح للصرف")]
        public decimal AvailableQuantity => Math.Max(0, PhysicalQuantity - ReservedQuantity);

        [Display(Name = "متوسط التكلفة المرجح (WAC)")]
        public decimal WeightedAverageCost { get; set; } = 0;

        [Display(Name = "تاريخ آخر تحديث")]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // العلاقات
        public virtual Warehouse Warehouse { get; set; }
        public virtual ItemMaster ItemMaster { get; set; }
    }

    // ============================================================
    //  سند صرف مخزني لمشروع مطبخ (StockIssue)
    // ============================================================
    public class StockIssue
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "طلب المطبخ")]
        public int KitchenRequestId { get; set; }

        [Display(Name = "أمر التنفيذ")]
        public int? WorkOrderId { get; set; }

        [Required]
        [Display(Name = "المستودع المصروف منه")]
        public int WarehouseId { get; set; }

        [Display(Name = "رقم سند الصرف")]
        [Required, StringLength(50)]
        public string IssueNumber { get; set; }

        [Display(Name = "تاريخ وساعة الصرف")]
        public DateTime IssuedAt { get; set; } = DateTime.Now;

        [Display(Name = "المستلم (الفني / الفريق)")]
        [Required, StringLength(100)]
        public string RecipientName { get; set; }

        [Display(Name = "إجمالي تكلفة المواد المصروفة (د.ل)")]
        public decimal TotalCost { get; set; } = 0;

        [Display(Name = "المعتمد")]
        public string ApprovedBy { get; set; }

        [Display(Name = "ملاحظات")]
        [StringLength(500)]
        public string Notes { get; set; }

        // العلاقات
        public virtual KitchenRequest KitchenRequest { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual ICollection<StockIssueItem> Items { get; set; } = new List<StockIssueItem>();
    }

    // ============================================================
    //  بند في سند الصرف المخزني (StockIssueItem)
    // ============================================================
    public class StockIssueItem
    {
        public int Id { get; set; }

        [Required]
        public int StockIssueId { get; set; }

        [Required]
        public int ItemMasterId { get; set; }

        [Display(Name = "الكمية المصروفة")]
        public decimal QuantityIssued { get; set; }

        [Display(Name = "تكلفة الوحدة عند الصرف (د.ل)")]
        public decimal UnitCost { get; set; }

        [Display(Name = "إجمالي تكلفة البند (د.ل)")]
        public decimal TotalCost => QuantityIssued * UnitCost;

        // العلاقات
        public virtual StockIssue StockIssue { get; set; }
        public virtual ItemMaster ItemMaster { get; set; }
    }

    // ============================================================
    //  سجل الحركات المخزنية العام (StockTransaction)
    // ============================================================
    public class StockTransaction
    {
        public int Id { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        [Required]
        public int ItemMasterId { get; set; }

        [Display(Name = "نوع الحركة")]
        public StockTransactionType TransactionType { get; set; }

        [Display(Name = "الكمية")]
        public decimal Quantity { get; set; }

        [Display(Name = "تكلفة الوحدة (د.ل)")]
        public decimal UnitCost { get; set; }

        [Display(Name = "رقم المستند المرجعي")]
        [StringLength(50)]
        public string ReferenceNumber { get; set; }

        [Display(Name = "تاريخ الحركة")]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Display(Name = "المستخدم المسؤول")]
        public string CreatedBy { get; set; }

        // العلاقات
        public virtual Warehouse Warehouse { get; set; }
        public virtual ItemMaster ItemMaster { get; set; }
    }
}
