using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOSS.Web.Models
{
    // ============================================================
    //  Ø­Ø§Ù„Ø© Ø£Ù…Ø± Ø§Ù„ØªÙ†ÙÙŠØ°
    // ============================================================
    public enum WorkOrderStatus
    {
        [Display(Name = "Ù‚ÙŠØ¯ Ø§Ù„ØªØ®Ø·ÙŠØ·")]
        Planning = 1,

        [Display(Name = "Ù‚ÙŠØ¯ Ø§Ù„ØªØµÙ†ÙŠØ¹ Ø¨Ø§Ù„Ù…ØµÙ†Ø¹")]
        Manufacturing = 2,

        [Display(Name = "ØªÙ… Ø§Ù„ØªØµÙ†ÙŠØ¹ - ÙØ­Øµ Ø§Ù„Ø¬ÙˆØ¯Ø©")]
        QualityInspection = 3,

        [Display(Name = "Ø¬Ø§Ù‡Ø² Ù„Ù„Ù†Ù‚Ù„ ÙˆØ§Ù„ØªØ±ÙƒÙŠØ¨")]
        ReadyForInstallation = 4,

        [Display(Name = "Ù‚ÙŠØ¯ Ø§Ù„ØªØ±ÙƒÙŠØ¨ Ø§Ù„Ù…ÙŠØ¯Ø§Ù†ÙŠ")]
        Installing = 5,

        [Display(Name = "Ù…Ø¹Ø§Ù„Ø¬Ø© Ù…Ù„Ø§Ø­Ø¸Ø§Øª ÙˆÙ†ÙˆØ§Ù‚Øµ")]
        SnagResolution = 6,

        [Display(Name = "Ø¬Ø§Ù‡Ø² Ù„Ù„ØªØ³Ù„ÙŠÙ… Ø§Ù„Ù†Ù‡Ø§Ø¦ÙŠ")]
        ReadyForHandover = 7,

        [Display(Name = "Ù…ÙƒØªÙ…Ù„ Ù†Ù‡Ø§Ø¦ÙŠØ§Ù‹")]
        Completed = 8,

        [Display(Name = "Ù…Ù„ØºÙ‰")]
        Cancelled = 9
    }

    // ============================================================
    //  Ø£Ù…Ø± Ø§Ù„ØªÙ†ÙÙŠØ° ÙˆØ§Ù„ØªØ´ØºÙŠÙ„ Ø§Ù„Ù…Ø±ÙƒØ²ÙŠ (WorkOrder)
    // ============================================================
    public class WorkOrder
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ø·Ù„Ø¨ Ø§Ù„Ù…Ø·Ø¨Ø®")]
        public int KitchenRequestId { get; set; }

        [Display(Name = "Ø§Ù„Ø¹Ù‚Ø¯ Ø§Ù„Ù…Ø±ØªØ¨Ø·")]
        public int? ContractId { get; set; }

        [Display(Name = "Ø±Ù‚Ù… Ø£Ù…Ø± Ø§Ù„ØªÙ†ÙÙŠØ°")]
        [StringLength(50)]
        public string OrderNumber { get; set; }

        [Display(Name = "Ø§Ù„Ø£ÙˆÙ„ÙˆÙŠØ©")]
        public PriorityLevel Priority { get; set; } = PriorityLevel.Normal;

        [Display(Name = "Ù…Ø³Ø¤ÙˆÙ„ Ø§Ù„Ø¥Ù†ØªØ§Ø¬ ÙˆØ§Ù„ØªÙ†ÙÙŠØ°")]
        public int? ProductionManagerId { get; set; }

        [Display(Name = "ØªØ§Ø±ÙŠØ® Ø§Ù„Ø¨Ø¯Ø¡ Ø§Ù„Ù…Ø®Ø·Ø·")]
        public DateTime? PlannedStartDate { get; set; }

        [Display(Name = "ØªØ§Ø±ÙŠØ® Ø§Ù„Ø§Ù†ØªÙ‡Ø§Ø¡ Ø§Ù„Ù…ØªÙˆÙ‚Ø¹")]
        public DateTime? ExpectedEndDate { get; set; }

        [Display(Name = "ØªØ§Ø±ÙŠØ® Ø§Ù„Ø§Ù†ØªÙ‡Ø§Ø¡ Ø§Ù„ÙØ¹Ù„ÙŠ")]
        public DateTime? ActualEndDate { get; set; }

        [Display(Name = "Ø­Ø§Ù„Ø© Ø£Ù…Ø± Ø§Ù„ØªÙ†ÙÙŠØ°")]
        public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Planning;

        [Display(Name = "Ù…Ù„Ø§Ø­Ø¸Ø§Øª ÙˆØªØ¹Ù„ÙŠÙ…Ø§Øª Ø§Ù„Ø¥Ù†ØªØ§Ø¬")]
        [StringLength(1000)]
        public string Notes { get; set; }

        [Display(Name = "ØªØ§Ø±ÙŠØ® Ø§Ù„Ø¥Ù†Ø´Ø§Ø¡")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ø£ÙÙ†Ø´Ø¦ Ø¨ÙˆØ§Ø³Ø·Ø©")]
        public string CreatedBy { get; set; }

        // Ø§Ù„Ø¹Ù„Ø§Ù‚Ø§Øª
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
    //  Ù‚Ø§Ø¦Ù…Ø© Ø§Ù„Ù…ÙˆØ§Ø¯ Ø§Ù„Ù…Ø·Ù„ÙˆØ¨Ø© Ù„Ø£Ù…Ø± Ø§Ù„ØªÙ†ÙÙŠØ° (MaterialRequirement - BOM)
    // ============================================================
    public class MaterialRequirement
    {
        public int Id { get; set; }

        [Required]
        public int WorkOrderId { get; set; }

        [Display(Name = "ÙƒÙˆØ¯ Ø§Ù„ØµÙ†Ù / Ø§Ù„Ù…Ø§Ø¯Ø©")]
        [StringLength(50)]
        public string ItemCode { get; set; }

        [Display(Name = "Ø§Ø³Ù… Ø§Ù„Ù…Ø§Ø¯Ø© / Ø§Ù„ØµÙ†Ù")]
        [Required, StringLength(200)]
        public string ItemName { get; set; }

        [Display(Name = "ÙØ¦Ø© Ø§Ù„Ù…Ø§Ø¯Ø©")]
        [StringLength(100)]
        public string Category { get; set; }

        [Display(Name = "Ø§Ù„ÙˆØ­Ø¯Ø©")]
        [StringLength(30)]
        public string Unit { get; set; } = "Ù‚Ø·Ø¹Ø©";

        [Display(Name = "Ø§Ù„ÙƒÙ…ÙŠØ© Ø§Ù„Ø¥Ø¬Ù…Ø§Ù„ÙŠØ© Ø§Ù„Ù…Ø·Ù„ÙˆØ¨Ø©")]
        public decimal QuantityRequired { get; set; }

        [Display(Name = "Ø§Ù„ÙƒÙ…ÙŠØ© Ø§Ù„Ù…Ø­Ø¬ÙˆØ²Ø© Ù…Ù† Ø§Ù„Ù…Ø®Ø²Ù†")]
        public decimal QuantityReserved { get; set; } = 0;

        [Display(Name = "Ø§Ù„ÙƒÙ…ÙŠØ© Ø§Ù„Ù…ØµØ±ÙˆÙØ© ÙØ¹Ù„ÙŠØ§Ù‹ Ù„Ù„Ù…Ø´Ø±ÙˆØ¹")]
        public decimal QuantityIssued { get; set; } = 0;

        [Display(Name = "Ø§Ù„ÙƒÙ…ÙŠØ© Ø§Ù„Ù†Ø§Ù‚ØµØ© Ø§Ù„Ù…Ø·Ù„ÙˆØ¨ Ø´Ø±Ø§Ø¤Ù‡Ø§")]
        public decimal QuantityToPurchase => Math.Max(0, QuantityRequired - (QuantityReserved + QuantityIssued));

        [Display(Name = "ØªÙƒÙ„ÙØ© Ø§Ù„ÙˆØ­Ø¯Ø© Ø§Ù„ØªÙ‚Ø¯ÙŠØ±ÙŠØ© (Ø¯.Ù„)")]
        public decimal EstimatedUnitCost { get; set; } = 0;

        [Display(Name = "Ø¥Ø¬Ù…Ø§Ù„ÙŠ Ø§Ù„ØªÙƒÙ„ÙØ© Ø§Ù„Ù…Ù‚Ø¯Ø±Ø© (Ø¯.Ù„)")]
        public decimal TotalEstimatedCost => QuantityRequired * EstimatedUnitCost;

        [Display(Name = "Ù‡Ù„ ØªÙ… Ø§ÙƒØªÙ…Ø§Ù„ ØµØ±Ù Ø§Ù„Ø¨Ù†Ø¯ØŸ")]
        public bool IsFullyIssued => QuantityIssued >= QuantityRequired;

        // Ø§Ù„Ø¹Ù„Ø§Ù‚Ø©
        public virtual WorkOrder WorkOrder { get; set; }
    }
}

