using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOSS.Web.Models
{
    // ============================================================
    //  Ø§Ù„Ù…ÙˆØ±Ø¯ (Supplier)
    // ============================================================
    public class Supplier
    {
        public int Id { get; set; }

        [Display(Name = "ÙƒÙˆØ¯ Ø§Ù„Ù…ÙˆØ±Ø¯")]
        [Required, StringLength(30)]
        public string Code { get; set; }

        [Display(Name = "Ø§Ø³Ù… Ø§Ù„Ù…ÙˆØ±Ø¯ / Ø§Ù„Ø´Ø±ÙƒØ©")]
        [Required, StringLength(150)]
        public string Name { get; set; }

        [Display(Name = "Ø±Ù‚Ù… Ø§Ù„Ù‡Ø§ØªÙ")]
        [Required, StringLength(30)]
        public string Phone { get; set; }

        [Display(Name = "Ø§Ù„Ø¨Ø±ÙŠØ¯ Ø§Ù„Ø¥Ù„ÙƒØªØ±ÙˆÙ†ÙŠ")]
        [StringLength(100)]
        public string Email { get; set; }

        [Display(Name = "Ø§Ù„Ø¹Ù†ÙˆØ§Ù†")]
        [StringLength(200)]
        public string Address { get; set; }

        [Display(Name = "Ø§Ù„Ø±ØµÙŠØ¯ Ø§Ù„Ù…Ø§Ù„ÙŠ Ø§Ù„Ø­Ø§Ù„ÙŠ Ø§Ù„Ù…Ø³ØªØ­Ù‚ Ù„Ù„Ù…ÙˆØ±Ø¯ (Ø¯.Ù„)")]
        public decimal CurrentBalance { get; set; } = 0;

        [Display(Name = "Ù‡Ù„ Ø§Ù„Ù…ÙˆØ±Ø¯ Ù†Ø´Ø·ØŸ")]
        public bool IsActive { get; set; } = true;

        // Ø§Ù„Ø¹Ù„Ø§Ù‚Ø§Øª
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }

    // ============================================================
    //  Ø·Ù„Ø¨ Ø§Ù„Ø§Ø­ØªÙŠØ§Ø¬ / Ø·Ù„Ø¨ Ø§Ù„Ø´Ø±Ø§Ø¡ Ø§Ù„Ø£ÙˆÙ„ÙŠ (PurchaseRequest)
    // ============================================================
    public class PurchaseRequest
    {
        public int Id { get; set; }

        [Display(Name = "Ø·Ù„Ø¨ Ø§Ù„Ù…Ø·Ø¨Ø® Ø§Ù„Ù…Ø±ØªØ¨Ø· (Ø¥Ù† ÙˆØ¬Ø¯)")]
        public int? KitchenRequestId { get; set; }

        [Display(Name = "Ø±Ù‚Ù… Ø·Ù„Ø¨ Ø§Ù„Ø§Ø­ØªÙŠØ§Ø¬")]
        [Required, StringLength(50)]
        public string RequestNumber { get; set; }

        [Display(Name = "Ø§Ù„ØºØ±Ø¶ Ù…Ù† Ø§Ù„Ø´Ø±Ø§Ø¡")]
        [Required, StringLength(200)]
        public string Purpose { get; set; } // Ù„Ù…Ø´Ø±ÙˆØ¹ Ù…Ø¹ÙŠÙ†ØŒ Ù„ØªØºØ°ÙŠØ© Ø§Ù„Ù…Ø®Ø²ÙˆÙ†...

        [Display(Name = "Ø§Ù„Ø£ÙˆÙ„ÙˆÙŠØ©")]
        public PriorityLevel Priority { get; set; } = PriorityLevel.Normal;

        [Display(Name = "ØªØ§Ø±ÙŠØ® Ø§Ù„Ø·Ù„Ø¨")]
        public DateTime RequestDate { get; set; } = DateTime.Now;

        [Display(Name = "Ø­Ø§Ù„Ø© Ø§Ù„Ø·Ù„Ø¨")]
        public string Status { get; set; } = "Pending"; // Pending, Approved, ConvertedToPO, Rejected

        [Display(Name = "Ø§Ù„Ù…Ø¹ØªÙ…Ø¯")]
        public string ApprovedBy { get; set; }

        [Display(Name = "ØªØ§Ø±ÙŠØ® Ø§Ù„Ø§Ø¹ØªÙ…Ø§Ø¯")]
        public DateTime? ApprovedAt { get; set; }

        // Ø§Ù„Ø¹Ù„Ø§Ù‚Ø§Øª
        public virtual KitchenRequest KitchenRequest { get; set; }
        public virtual ICollection<PurchaseRequestItem> Items { get; set; } = new List<PurchaseRequestItem>();
    }

    // ============================================================
    //  Ø¨Ù†Ø¯ Ø·Ù„Ø¨ Ø§Ù„Ø§Ø­ØªÙŠØ§Ø¬ (PurchaseRequestItem)
    // ============================================================
    public class PurchaseRequestItem
    {
        public int Id { get; set; }

        [Required]
        public int PurchaseRequestId { get; set; }

        [Required]
        public int ItemMasterId { get; set; }

        [Display(Name = "Ø§Ù„ÙƒÙ…ÙŠØ© Ø§Ù„Ù…Ø·Ù„ÙˆØ¨Ø©")]
        public decimal QuantityRequested { get; set; }

        [Display(Name = "Ù…Ù„Ø§Ø­Ø¸Ø§Øª Ø§Ù„Ø¨Ù†Ø¯")]
        [StringLength(200)]
        public string Notes { get; set; }

        // Ø§Ù„Ø¹Ù„Ø§Ù‚Ø§Øª
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
        public string Status { get; set; } = "Draft"; // Draft, Approved, SentToSupplier, PartiallyReceived, Completed, Cancelled

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

    // ============================================================
    //  بند في أمر الشراء (PurchaseOrderItem)
    // ============================================================
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

        [Display(Name = "Ø±Ù‚Ù… Ø³Ù†Ø¯ Ø§Ù„Ø§Ø³ØªÙ„Ø§Ù…")]
        [Required, StringLength(50)]
        public string ReceiptNumber { get; set; }

        [Display(Name = "ØªØ§Ø±ÙŠØ® Ø§Ù„Ø§Ø³ØªÙ„Ø§Ù…")]
        public DateTime ReceivedDate { get; set; } = DateTime.Now;

        [Display(Name = "Ø£Ù…ÙŠÙ† Ø§Ù„Ù…Ø®Ø²Ù† Ø§Ù„Ù…Ø³ØªÙ„Ù…")]
        public string ReceivedBy { get; set; }

        [Display(Name = "Ø±Ù‚Ù… Ø¥Ø°Ù† ØªØ³Ù„ÙŠÙ… / ÙØ§ØªÙˆØ±Ø© Ø§Ù„Ù…ÙˆØ±Ø¯")]
        [StringLength(100)]
        public string SupplierDeliveryNote { get; set; }

        [Display(Name = "Ø­Ø§Ù„Ø© Ø§Ù„Ø§Ø³ØªÙ„Ø§Ù… ÙˆØ§Ù„ÙØ­Øµ")]
        public string QualityStatus { get; set; } = "Passed"; // Passed, PartialPass, Rejected

        [Display(Name = "Ù…Ù„Ø§Ø­Ø¸Ø§Øª Ø§Ù„ÙØ­Øµ ÙˆØ§Ù„Ø§Ø³ØªÙ„Ø§Ù…")]
        [StringLength(500)]
        public string Notes { get; set; }

        // Ø§Ù„Ø¹Ù„Ø§Ù‚Ø§Øª
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual ICollection<GoodsReceiptItem> Items { get; set; } = new List<GoodsReceiptItem>();
    }

    // ============================================================
    //  Ø¨Ù†Ø¯ ÙÙŠ Ø³Ù†Ø¯ Ø§Ø³ØªÙ„Ø§Ù… Ø§Ù„Ø¨Ø¶Ø§Ø¹Ø© (GoodsReceiptItem)
    // ============================================================
    public class GoodsReceiptItem
    {
        public int Id { get; set; }

        [Required]
        public int GoodsReceiptId { get; set; }

        [Required]
        public int ItemMasterId { get; set; }

        [Display(Name = "Ø§Ù„ÙƒÙ…ÙŠØ© Ø§Ù„Ù…Ø³ØªÙ„Ù…Ø© Ø§Ù„Ø³Ù„ÙŠÙ…Ø©")]
        public decimal QuantityReceived { get; set; }

        [Display(Name = "Ø§Ù„ÙƒÙ…ÙŠØ© Ø§Ù„ØªØ§Ù„ÙØ© / Ø§Ù„Ù…Ø±ÙÙˆØ¶Ø©")]
        public decimal QuantityDamaged { get; set; } = 0;

        [Display(Name = "Ø³Ø¹Ø± Ø§Ù„ÙˆØ­Ø¯Ø© Ù…Ù† Ø§Ù„ÙØ§ØªÙˆØ±Ø© (Ø¯.Ù„)")]
        public decimal UnitCost { get; set; }

        [Display(Name = "Ø§Ù„Ø¥Ø¬Ù…Ø§Ù„ÙŠ (Ø¯.Ù„)")]
        public decimal TotalCost => QuantityReceived * UnitCost;

        // Ø§Ù„Ø¹Ù„Ø§Ù‚Ø§Øª
        public virtual GoodsReceipt GoodsReceipt { get; set; }
        public virtual ItemMaster ItemMaster { get; set; }
    }
}

