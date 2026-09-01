using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOSS.Web.Models
{
    // ============================================================
    //  المورد (Supplier)
    // ============================================================
    public class Supplier
    {
        public int Id { get; set; }

        [Display(Name = "كود المورد")]
        [Required, StringLength(30)]
        public string Code { get; set; }

        [Display(Name = "اسم المورد / الشركة")]
        [Required, StringLength(150)]
        public string Name { get; set; }

        [Display(Name = "رقم الهاتف")]
        [Required, StringLength(30)]
        public string Phone { get; set; }

        [Display(Name = "البريد الإلكتروني")]
        [StringLength(100)]
        public string Email { get; set; }

        [Display(Name = "العنوان")]
        [StringLength(200)]
        public string Address { get; set; }

        [Display(Name = "الرصيد المالي الحالي المستحق للمورد (د.ل)")]
        public decimal CurrentBalance { get; set; } = 0;

        [Display(Name = "هل المورد نشط؟")]
        public bool IsActive { get; set; } = true;

        // العلاقات
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }

    // ============================================================
    //  طلب الاحتياج / طلب الشراء الأولي (PurchaseRequest)
    // ============================================================
    public enum PriorityLevel
    {
        [Display(Name = "عادي")]
        Normal = 1,

        [Display(Name = "عاجل")]
        Urgent = 2,

        [Display(Name = "طارئ جداً")]
        Critical = 3
    }

    public class PurchaseRequest
    {
        public int Id { get; set; }

        [Display(Name = "طلب المطبخ المرتبط")]
        public int? KitchenRequestId { get; set; }

        [Display(Name = "رقم طلب الاحتياج")]
        [Required, StringLength(50)]
        public string RequestNumber { get; set; }

        [Display(Name = "الغرض من الشراء")]
        [Required, StringLength(200)]
        public string Purpose { get; set; }

        [Display(Name = "الأولوية")]
        public PriorityLevel Priority { get; set; } = PriorityLevel.Normal;

        [Display(Name = "تاريخ الطلب")]
        public DateTime RequestDate { get; set; } = DateTime.Now;

        [Display(Name = "حالة الطلب")]
        public string Status { get; set; } = "Pending";

        [Display(Name = "المعتمد")]
        public string ApprovedBy { get; set; }

        [Display(Name = "تاريخ الاعتماد")]
        public DateTime? ApprovedAt { get; set; }

        // العلاقات
        public virtual KitchenRequest KitchenRequest { get; set; }
        public virtual ICollection<PurchaseRequestItem> Items { get; set; } = new List<PurchaseRequestItem>();
    }

    public class PurchaseRequestItem
    {
        public int Id { get; set; }

        [Required]
        public int PurchaseRequestId { get; set; }

        [Required]
        public int ItemMasterId { get; set; }

        [Display(Name = "الكمية المطلوبة")]
        public decimal QuantityRequested { get; set; }

        [Display(Name = "ملاحظات البند")]
        [StringLength(200)]
        public string Notes { get; set; }

        // العلاقات
        public virtual PurchaseRequest PurchaseRequest { get; set; }
        public virtual ItemMaster ItemMaster { get; set; }
    }

    // ============================================================
    //  أمر الشراء للمورد (PurchaseOrder)
    // ============================================================
    public class PurchaseOrder
    {
        public int Id { get; set; }

        [Display(Name = "المورد")]
        public int? SupplierId { get; set; }

        [Display(Name = "طلب المطبخ المرتبط")]
        public int? KitchenRequestId { get; set; }

        [Display(Name = "رقم أمر الشراء")]
        [Required, StringLength(50)]
        public string OrderNumber { get; set; }

        [Display(Name = "تاريخ أمر الشراء")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Display(Name = "الموعد المتوقع للاستلام")]
        public DateTime? ExpectedDeliveryDate { get; set; }

        [Display(Name = "إجمالي قيمة أمر الشراء (د.ل)")]
        public decimal TotalAmount { get; set; } = 0;

        [Display(Name = "حالة أمر الشراء")]
        public string Status { get; set; } = "Draft";

        [Display(Name = "ملاحظات وشروط التوريد")]
        [StringLength(500)]
        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }

        // العلاقات
        public virtual Supplier Supplier { get; set; }
        public virtual KitchenRequest KitchenRequest { get; set; }
        public virtual ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
        public virtual ICollection<GoodsReceipt> GoodsReceipts { get; set; } = new List<GoodsReceipt>();
    }

    public class PurchaseOrderItem
    {
        public int Id { get; set; }

        [Required]
        public int PurchaseOrderId { get; set; }

        [Required]
        public int ItemMasterId { get; set; }

        [Display(Name = "الكمية المطلوبة")]
        public decimal Quantity { get; set; }

        [Display(Name = "سعر الوحدة من المورد (د.ل)")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "الإجمالي (د.ل)")]
        public decimal TotalPrice => Quantity * UnitPrice;

        // العلاقات
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public virtual ItemMaster ItemMaster { get; set; }
    }

    // ============================================================
    //  سند استلام البضاعة بالمخزن (GoodsReceipt)
    // ============================================================
    public class GoodsReceipt
    {
        public int Id { get; set; }

        [Required]
        public int PurchaseOrderId { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        [Display(Name = "رقم سند الاستلام")]
        [Required, StringLength(50)]
        public string ReceiptNumber { get; set; }

        [Display(Name = "تاريخ الاستلام")]
        public DateTime ReceivedDate { get; set; } = DateTime.Now;

        [Display(Name = "أمين المخزن المستلم")]
        public string ReceivedBy { get; set; }

        [Display(Name = "رقم إذن تسليم / فاتورة المورد")]
        [StringLength(100)]
        public string SupplierDeliveryNote { get; set; }

        [Display(Name = "حالة الاستلام والفحص")]
        public string QualityStatus { get; set; } = "Passed";

        [Display(Name = "ملاحظات الفحص والاستلام")]
        [StringLength(500)]
        public string Notes { get; set; }

        // العلاقات
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual ICollection<GoodsReceiptItem> Items { get; set; } = new List<GoodsReceiptItem>();
    }

    public class GoodsReceiptItem
    {
        public int Id { get; set; }

        [Required]
        public int GoodsReceiptId { get; set; }

        [Required]
        public int ItemMasterId { get; set; }

        [Display(Name = "الكمية المستلمة السليمة")]
        public decimal QuantityReceived { get; set; }

        [Display(Name = "الكمية التالفة / المرفوضة")]
        public decimal QuantityDamaged { get; set; } = 0;

        [Display(Name = "سعر الوحدة من الفاتورة (د.ل)")]
        public decimal UnitCost { get; set; }

        [Display(Name = "الإجمالي (د.ل)")]
        public decimal TotalCost => QuantityReceived * UnitCost;

        // العلاقات
        public virtual GoodsReceipt GoodsReceipt { get; set; }
        public virtual ItemMaster ItemMaster { get; set; }
    }
}
