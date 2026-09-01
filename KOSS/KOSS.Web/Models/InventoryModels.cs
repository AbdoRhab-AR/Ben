using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOSS.Web.Models
{
    // ============================================================
    //  Ø£Ù†ÙˆØ§Ø¹ Ø§Ù„Ø­Ø±ÙƒØ§Øª Ø§Ù„Ù…Ø®Ø²Ù†ÙŠØ©
    // ============================================================
    public enum StockTransactionType
    {
        [Display(Name = "Ø§Ø³ØªÙ„Ø§Ù… Ù…Ø®Ø²Ù†ÙŠ (Ø´Ø±Ø§Ø¡ / ØªÙˆØ±ÙŠØ¯)")]
        Receipt = 1,

        [Display(Name = "ØµØ±Ù Ù„Ù…Ø´Ø±ÙˆØ¹ Ù…Ø·Ø¨Ø®")]
        IssueToProject = 2,

        [Display(Name = "ØµØ±Ù ØªØ´ØºÙŠÙ„ÙŠ Ø¹Ø§Ù…")]
        IssueGeneral = 3,

        [Display(Name = "Ù…Ù†Ø§Ù‚Ù„Ø© Ø¨ÙŠÙ† Ù…Ø³ØªÙˆØ¯Ø¹ÙŠÙ†")]
        Transfer = 4,

        [Display(Name = "Ù…Ø±ØªØ¬Ø¹ Ù…Ù† Ù…Ø´Ø±ÙˆØ¹ Ù„Ù„Ù…Ø®Ø²Ù†")]
        ReturnFromProject = 5,

        [Display(Name = "Ù…Ø±ØªØ¬Ø¹ Ù„Ù…ÙˆØ±Ø¯")]
        ReturnToSupplier = 6,

        [Display(Name = "ØªØ³ÙˆÙŠØ© Ø¬Ø±Ø¯ÙŠØ© (+ / -)")]
        InventoryAdjustment = 7
    }

    // ============================================================
    //  Ø§Ù„Ù…Ø³ØªÙˆØ¯Ø¹ (Warehouse)
    // ============================================================
    public class Warehouse
    {
        public int Id { get; set; }

        [Display(Name = "ÙƒÙˆØ¯ Ø§Ù„Ù…Ø³ØªÙˆØ¯Ø¹")]
        [Required, StringLength(30)]
        public string Code { get; set; }

        [Display(Name = "Ø§Ø³Ù… Ø§Ù„Ù…Ø³ØªÙˆØ¯Ø¹")]
        [Required, StringLength(150)]
        public string Name { get; set; }

        [Display(Name = "Ø§Ù„Ù…ÙˆÙ‚Ø¹ Ø§Ù„Ø¬ØºØ±Ø§ÙÙŠ")]
        [StringLength(200)]
        public string Location { get; set; }

        [Display(Name = "Ø£Ù…ÙŠÙ† Ø§Ù„Ù…Ø³ØªÙˆØ¯Ø¹")]
        [StringLength(100)]
        public string KeeperName { get; set; }

        [Display(Name = "Ù‡Ù„ Ø§Ù„Ù…Ø³ØªÙˆØ¯Ø¹ Ù†Ø´Ø·ØŸ")]
        public bool IsActive { get; set; } = true;

        // Ø§Ù„Ø¹Ù„Ø§Ù‚Ø§Øª
        public virtual ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();
        public virtual ICollection<StockIssue> StockIssues { get; set; } = new List<StockIssue>();
    }

    // ============================================================
    //  Ø¯Ù„ÙŠÙ„ Ø§Ù„Ø£ØµÙ†Ø§Ù ÙˆØ§Ù„Ù…ÙˆØ§Ø¯ Ø§Ù„Ù‚ÙŠØ§Ø³ÙŠ (ItemMaster)
    // ============================================================
    public class ItemMaster
    {
        public int Id { get; set; }

        [Display(Name = "ÙƒÙˆØ¯ Ø§Ù„ØµÙ†Ù Ø§Ù„Ù…Ø¹ÙŠØ§Ø±ÙŠ")]
        [Required, StringLength(50)]
        public string ItemCode { get; set; }

        [Display(Name = "Ø§Ø³Ù… Ø§Ù„Ù…Ø§Ø¯Ø© / Ø§Ù„ØµÙ†Ù")]
        [Required, StringLength(200)]
        public string Name { get; set; }

        [Display(Name = "Ø§Ù„ÙØ¦Ø© Ø§Ù„Ø±Ø¦ÙŠØ³ÙŠØ©")]
        [Required, StringLength(100)]
        public string Category { get; set; } // Ø£Ù„ÙˆØ§Ø­ Ø®Ø´Ø¨ØŒ Ù‚ÙˆØ§Ø·Ø¹ØŒ Ù…ÙØµÙ„Ø§ØªØŒ Ø³ÙƒÙƒ Ø£Ø¯Ø±Ø§Ø¬ØŒ Ù…Ù‚Ø§Ø¨Ø¶ØŒ Ø­ÙˆØ§Ù PVCØŒ Ø±Ø®Ø§Ù…...

        [Display(Name = "Ø§Ù„ÙˆØ­Ø¯Ø© Ø§Ù„Ù‚ÙŠØ§Ø³ÙŠØ©")]
        [Required, StringLength(30)]
        public string Unit { get; set; } = "Ù‚Ø·Ø¹Ø©";

        [Display(Name = "Ø§Ù„ØªÙƒÙ„ÙØ© Ø§Ù„Ù‚ÙŠØ§Ø³ÙŠØ© (Ø¯.Ù„)")]
        public decimal StandardCost { get; set; } = 0;

        [Display(Name = "Ø³Ø¹Ø± Ø§Ù„Ø¨ÙŠØ¹ Ø§Ù„Ù‚ÙŠØ§Ø³ÙŠ (Ø¯.Ù„)")]
        public decimal StandardSalePrice { get; set; } = 0;

        [Display(Name = "Ø­Ø¯ Ø¥Ø¹Ø§Ø¯Ø© Ø§Ù„Ø·Ù„Ø¨ Ø§Ù„Ø£Ø¯Ù†Ù‰")]
        public decimal ReorderLevel { get; set; } = 5;

        [Display(Name = "Ø§Ù„Ù…ÙˆØ§ØµÙØ§Øª Ø§Ù„ÙÙ†ÙŠØ©")]
        [StringLength(500)]
        public string Specifications { get; set; }

        [Display(Name = "Ù‡Ù„ Ø§Ù„ØµÙ†Ù Ù†Ø´Ø·ØŸ")]
        public bool IsActive { get; set; } = true;

        // Ø§Ù„Ø¹Ù„Ø§Ù‚Ø§Øª
        public virtual ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();
    }

    // ============================================================
    //  Ø±ØµÙŠØ¯ Ø§Ù„ØµÙ†Ù Ø¨Ø§Ù„Ù…Ø³ØªÙˆØ¯Ø¹ (StockItem) - Ø§Ù„Ø±ØµÙŠØ¯ Ø§Ù„ÙØ¹Ù„ÙŠ ÙˆØ§Ù„Ù…Ø­Ø¬ÙˆØ² ÙˆØ§Ù„Ù…ØªØ§Ø­
    // ============================================================
    public class StockItem
    {
        public int Id { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        [Required]
        public int ItemMasterId { get; set; }

        [Display(Name = "Ø§Ù„Ø±ØµÙŠØ¯ Ø§Ù„ÙØ¹Ù„ÙŠ (Physical Stock)")]
        public decimal PhysicalQuantity { get; set; } = 0;

        [Display(Name = "Ø§Ù„Ø±ØµÙŠØ¯ Ø§Ù„Ù…Ø­Ø¬ÙˆØ² Ù„Ù„Ù…Ø´Ø§Ø±ÙŠØ¹ (Reserved Stock)")]
        public decimal ReservedQuantity { get; set; } = 0;

        [Display(Name = "Ø§Ù„Ø±ØµÙŠØ¯ Ø§Ù„Ù…ØªØ§Ø­ Ù„Ù„ØµØ±Ù (Available Stock)")]
        public decimal AvailableQuantity => Math.Max(0, PhysicalQuantity - ReservedQuantity);

        [Display(Name = "Ù…ØªÙˆØ³Ø· Ø§Ù„ØªÙƒÙ„ÙØ© Ø§Ù„Ù…Ø±Ø¬Ø­ (WAC)")]
        public decimal WeightedAverageCost { get; set; } = 0;

        [Display(Name = "ØªØ§Ø±ÙŠØ® Ø¢Ø®Ø± Ø¬Ø±Ø¯ / ØªØ­Ø¯ÙŠØ«")]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // Ø§Ù„Ø¹Ù„Ø§Ù‚Ø§Øª
        public virtual Warehouse Warehouse { get; set; }
        public virtual ItemMaster ItemMaster { get; set; }
    }

    // ============================================================
    //  Ø³Ù†Ø¯ ØµØ±Ù Ù…Ø®Ø²Ù†ÙŠ Ù„Ù…Ø´Ø±ÙˆØ¹ Ù…Ø·Ø¨Ø® (StockIssue)
    // ============================================================
    public class StockIssue
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ø·Ù„Ø¨ Ø§Ù„Ù…Ø·Ø¨Ø® / Ù…Ø±ÙƒØ² Ø§Ù„ØªÙƒÙ„ÙØ©")]
        public int KitchenRequestId { get; set; }

        [Display(Name = "Ø£Ù…Ø± Ø§Ù„ØªÙ†ÙÙŠØ°")]
        public int? WorkOrderId { get; set; }

        [Required]
        [Display(Name = "Ø§Ù„Ù…Ø³ØªÙˆØ¯Ø¹ Ø§Ù„Ù…ØµØ±ÙˆÙ Ù…Ù†Ù‡")]
        public int WarehouseId { get; set; }

        [Display(Name = "Ø±Ù‚Ù… Ø³Ù†Ø¯ Ø§Ù„ØµØ±Ù")]
        [Required, StringLength(50)]
        public string IssueNumber { get; set; }

        [Display(Name = "ØªØ§Ø±ÙŠØ® ÙˆØ³Ø§Ø¹Ø© Ø§Ù„ØµØ±Ù")]
        public DateTime IssuedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ø§Ù„Ù…Ø³ØªÙ„Ù… (Ø§Ù„ÙÙ†ÙŠ / Ø§Ù„ÙØ±ÙŠÙ‚)")]
        [Required, StringLength(100)]
        public string RecipientName { get; set; }

        [Display(Name = "Ø¥Ø¬Ù…Ø§Ù„ÙŠ ØªÙƒÙ„ÙØ© Ø§Ù„Ù…ÙˆØ§Ø¯ Ø§Ù„Ù…ØµØ±ÙˆÙØ© (Ø¯.Ù„)")]
        public decimal TotalCost { get; set; } = 0;

        [Display(Name = "Ø§Ù„Ù…Ø¹ØªÙ…Ø¯")]
        public string ApprovedBy { get; set; }

        [Display(Name = "Ù…Ù„Ø§Ø­Ø¸Ø§Øª")]
        [StringLength(500)]
        public string Notes { get; set; }

        // Ø§Ù„Ø¹Ù„Ø§Ù‚Ø§Øª
        public virtual KitchenRequest KitchenRequest { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual ICollection<StockIssueItem> Items { get; set; } = new List<StockIssueItem>();
    }

    // ============================================================
    //  Ø¨Ù†Ø¯ ÙÙŠ Ø³Ù†Ø¯ Ø§Ù„ØµØ±Ù Ø§Ù„Ù…Ø®Ø²Ù†ÙŠ (StockIssueItem)
    // ============================================================
    public class StockIssueItem
    {
        public int Id { get; set; }

        [Required]
        public int StockIssueId { get; set; }

        [Required]
        public int ItemMasterId { get; set; }

        [Display(Name = "Ø§Ù„ÙƒÙ…ÙŠØ© Ø§Ù„Ù…ØµØ±ÙˆÙØ©")]
        public decimal QuantityIssued { get; set; }

        [Display(Name = "ØªÙƒÙ„ÙØ© Ø§Ù„ÙˆØ­Ø¯Ø© Ø¹Ù†Ø¯ Ø§Ù„ØµØ±Ù (Ø¯.Ù„)")]
        public decimal UnitCost { get; set; }

        [Display(Name = "Ø¥Ø¬Ù…Ø§Ù„ÙŠ ØªÙƒÙ„ÙØ© Ø§Ù„Ø¨Ù†Ø¯ (Ø¯.Ù„)")]
        public decimal TotalCost => QuantityIssued * UnitCost;

        // Ø§Ù„Ø¹Ù„Ø§Ù‚Ø§Øª
        public virtual StockIssue StockIssue { get; set; }
        public virtual ItemMaster ItemMaster { get; set; }
    }

    // ============================================================
    //  Ø³Ø¬Ù„ Ø§Ù„Ø­Ø±ÙƒØ§Øª Ø§Ù„Ù…Ø®Ø²Ù†ÙŠØ© Ø§Ù„Ø¹Ø§Ù… (StockTransaction)
    // ============================================================
    public class StockTransaction
    {
        public int Id { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        [Required]
        public int ItemMasterId { get; set; }

        [Display(Name = "Ù†ÙˆØ¹ Ø§Ù„Ø­Ø±ÙƒØ©")]
        public StockTransactionType TransactionType { get; set; }

        [Display(Name = "Ø±Ù‚Ù… Ø§Ù„Ù…Ø³ØªÙ†Ø¯ Ø§Ù„Ù…Ø±Ø¬Ø¹ÙŠ")]
        [StringLength(100)]
        public string ReferenceNumber { get; set; }

        [Display(Name = "Ù…Ø±ÙƒØ² Ø§Ù„ØªÙƒÙ„ÙØ© / Ø·Ù„Ø¨ Ø§Ù„Ù…Ø·Ø¨Ø®")]
        public int? KitchenRequestId { get; set; }

        [Display(Name = "Ø§Ù„ÙƒÙ…ÙŠØ© Ø§Ù„ÙˆØ§Ø±Ø¯Ø© (+)")]
        public decimal InQuantity { get; set; } = 0;

        [Display(Name = "Ø§Ù„ÙƒÙ…ÙŠØ© Ø§Ù„ØµØ§Ø¯Ø±Ø© (-)")]
        public decimal OutQuantity { get; set; } = 0;

        [Display(Name = "Ø³Ø¹Ø± Ø§Ù„ÙˆØ­Ø¯Ø© Ù„Ù„Ø­Ø±ÙƒØ© (Ø¯.Ù„)")]
        public decimal UnitCost { get; set; } = 0;

        [Display(Name = "Ø§Ù„Ù‚ÙŠÙ…Ø© Ø§Ù„Ø¥Ø¬Ù…Ø§Ù„ÙŠØ© Ù„Ù„Ø­Ø±ÙƒØ© (Ø¯.Ù„)")]
        public decimal TotalCost => (InQuantity > 0 ? InQuantity : OutQuantity) * UnitCost;

        [Display(Name = "ØªØ§Ø±ÙŠØ® Ø§Ù„Ø­Ø±ÙƒØ©")]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Display(Name = "Ø§Ù„Ù…Ø³ØªØ®Ø¯Ù… Ø§Ù„Ø°ÙŠ Ù†ÙØ° Ø§Ù„Ø­Ø±ÙƒØ©")]
        public string CreatedBy { get; set; }

        // Ø§Ù„Ø¹Ù„Ø§Ù‚Ø§Øª
        public virtual Warehouse Warehouse { get; set; }
        public virtual ItemMaster ItemMaster { get; set; }
        public virtual KitchenRequest KitchenRequest { get; set; }
    }
}

