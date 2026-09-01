using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace KOSS.Web.Models
{
    // ============================================================
    //  Ø­Ø§Ù„Ø© Ø§Ù„Ø¹Ù‚Ø¯
    // ============================================================
    public enum ContractStatus
    {
        [Display(Name = "Ø¬Ø¯ÙŠØ¯")]
        New = 1,

        [Display(Name = "ØªÙ…Øª Ø§Ù„Ù…Ø¹Ø§ÙŠÙ†Ø©")]
        Measured = 2,

        [Display(Name = "ØªÙ… Ø¯ÙØ¹ Ø±Ø³ÙˆÙ… Ø§Ù„ØªØµÙ…ÙŠÙ…")]
        FeePaid = 3,

        [Display(Name = "ØªÙ… Ø§Ù„ØªØµÙ…ÙŠÙ…")]
        Designed = 4,

        [Display(Name = "ØªÙ… Ø¯ÙØ¹ Ø§Ù„Ø¹Ø±Ø¨ÙˆÙ†")]
        DepositPaid = 5,

        [Display(Name = "Ù‚ÙŠØ¯ Ø§Ù„ØªØµÙ†ÙŠØ¹")]
        UnderProduction = 6,

        [Display(Name = "ØªÙ… Ø§Ù„ØªØµÙ†ÙŠØ¹")]
        Manufactured = 7,

        [Display(Name = "ØªÙ… Ø§Ù„ØªØ±ÙƒÙŠØ¨")]
        Installed = 8,

        [Display(Name = "Ù‚ÙŠØ¯ Ø§Ù„ØªØ³Ù„ÙŠÙ…")]
        Commissioning = 9,

        [Display(Name = "Ù…ÙƒØªÙ…Ù„")]
        Completed = 10,

        [Display(Name = "Ù…Ù„ØºÙ‰")]
        Cancelled = 11,

        [Display(Name = "Ù…Ø³ÙˆØ¯Ø© Ø¹Ù‚Ø¯")]
        Draft = 12,

        [Display(Name = "Ø¨Ø§Ù†ØªØ¸Ø§Ø± Ø³Ø¯Ø§Ø¯ Ø§Ù„Ø¹Ø±Ø¨ÙˆÙ†")]
        AwaitingDeposit = 13,

        [Display(Name = "Ø¹Ù‚Ø¯ Ù†Ø´Ø· ÙˆØ³Ø§Ø±Ù")]
        Active = 14,

        [Display(Name = "Ù…Ø¹Ù„Ù‚ / Ù…ÙˆÙ‚ÙˆÙ")]
        Suspended = 15,

        [Display(Name = "Ù…Ù„Ø­Ù‚ Ù…Ø¹Ø¯Ù„")]
        Amended = 16,

        [Display(Name = "Ù…Ù„ØºÙ‰ / Ù…Ù†Ø³ÙˆØ®")]
        Terminated = 17
    }

    // ============================================================
    //  Ø§Ù„Ø¹Ù‚Ø¯ Ø§Ù„Ø±Ø³Ù…ÙŠ (Contract)
    // ============================================================
    public class Contract
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ø·Ù„Ø¨ Ø§Ù„Ù…Ø·Ø¨Ø®")]
        public int KitchenRequestId { get; set; }

        [Display(Name = "Ø§Ù„Ø¹Ù…ÙŠÙ„")]
        public int? ClientId { get; set; }

        [Display(Name = "Ø¹Ø±Ø¶ Ø§Ù„Ø³Ø¹Ø± Ø§Ù„Ù…Ø¹ØªÙ…Ø¯")]
        public int? QuotationId { get; set; }

        [Display(Name = "Ø¥ØµØ¯Ø§Ø± Ø§Ù„ØªØµÙ…ÙŠÙ… Ø§Ù„Ù…Ø¹ØªÙ…Ø¯")]
        public int? DesignVersionId { get; set; }

        [Display(Name = "Ø±Ù‚Ù… Ø§Ù„Ø¹Ù‚Ø¯")]
        [StringLength(50)]
        public string ContractNumber { get; set; }

        [Display(Name = "Ø§Ù„Ù‚ÙŠÙ…Ø© Ø§Ù„Ø¥Ø¬Ù…Ø§Ù„ÙŠØ© Ù„Ù„Ø¹Ù‚Ø¯ (Ø¯.Ù„)")]
        public decimal TotalValue { get; set; }

        [Display(Name = "Ø§Ù„Ø¹Ø±Ø¨ÙˆÙ† Ø§Ù„Ù…Ø·Ù„ÙˆØ¨ Ù„Ù„ØªÙØ¹ÙŠÙ„ (Ø¯.Ù„)")]
        public decimal RequiredDeposit { get; set; }

        [Display(Name = "Ø¥Ø¬Ù…Ø§Ù„ÙŠ Ø§Ù„Ù…Ù‚Ø¨ÙˆØ¶Ø§Øª (Ø¯.Ù„)")]
        public decimal TotalPaid { get; set; } = 0;

        [Display(Name = "Ø§Ù„Ù…ØªØ¨Ù‚ÙŠ Ù„Ù„ØªØ­ØµÙŠÙ„ (Ø¯.Ù„)")]
        public decimal RemainingBalance => Math.Max(0, TotalValue - TotalPaid);

        [Display(Name = "Ù†Ø³Ø¨Ø© Ø§Ù„Ø³Ø¯Ø§Ø¯ (%)")]
        public decimal PaymentPercentage => TotalValue > 0 ? (TotalPaid / TotalValue) * 100 : 0;

        [Display(Name = "Ø³Ø¹Ø± Ø§Ù„Ù…ØªØ± Ø§Ù„Ù…Ø¹ØªÙ…Ø¯ (Ø¯.Ù„)")]
        public decimal PricePerMeter { get; set; }

        [Display(Name = "Ø¥Ø¬Ù…Ø§Ù„ÙŠ Ø§Ù„Ø£Ù…ØªØ§Ø± Ø§Ù„Ù…Ø¹ØªÙ…Ø¯Ø©")]
        public decimal TotalMeters { get; set; }

        [Display(Name = "ØªØ§Ø±ÙŠØ® ØªÙˆÙ‚ÙŠØ¹ Ø§Ù„Ø¹Ù‚Ø¯")]
        public DateTime? SignedDate { get; set; }

        [Display(Name = "ØªØ§Ø±ÙŠØ® Ø§Ù„ØªØ³Ù„ÙŠÙ… Ø§Ù„Ù…ØªÙÙ‚ Ø¹Ù„ÙŠÙ‡")]
        public DateTime? TargetCompletionDate { get; set; }

        [Display(Name = "Ø§Ù„Ø´Ø±Ø· Ø§Ù„Ø¬Ø²Ø§Ø¦ÙŠ Ø¹Ù† ÙƒÙ„ ÙŠÙˆÙ… ØªØ£Ø®ÙŠØ± (Ø¯.Ù„)")]
        public decimal PenaltyPerDay { get; set; } = 0;

        [Display(Name = "Ø±Ø§Ø¨Ø· Ù…Ù„Ù Ø§Ù„Ø¹Ù‚Ø¯ Ø§Ù„Ù…ÙˆÙ‚Ø¹ Ø§Ù„Ù…Ù…Ø³ÙˆØ­ Ø¶ÙˆØ¦ÙŠØ§Ù‹")]
        [StringLength(500)]
        public string SignedContractFilePath { get; set; }

        [Display(Name = "Ø­Ø§Ù„Ø© Ø§Ù„Ø¹Ù‚Ø¯")]
        public ContractStatus Status { get; set; } = ContractStatus.Draft;

        [Display(Name = "Ø´Ø±ÙˆØ· ÙˆÙ…Ù„Ø§Ø­Ø¸Ø§Øª Ø§Ù„Ø¹Ù‚Ø¯")]
        [StringLength(1000)]
        public string Notes { get; set; }

        [Display(Name = "ØªØ§Ø±ÙŠØ® Ø§Ù„Ø¥Ù†Ø´Ø§Ø¡")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "ØªØ§Ø±ÙŠØ® Ø¢Ø®Ø± ØªØ¹Ø¯ÙŠÙ„")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ø£ÙÙ†Ø´Ø¦ Ø¨ÙˆØ§Ø³Ø·Ø©")]
        public string CreatedBy { get; set; }

        // Ø§Ù„Ø¹Ù„Ø§Ù‚Ø§Øª
        public virtual KitchenRequest KitchenRequest { get; set; }
        public virtual Client Client { get; set; }
        public virtual Quotation Quotation { get; set; }
        public virtual DesignVersion DesignVersion { get; set; }

        public virtual ICollection<PaymentSchedule> PaymentSchedules { get; set; } = new List<PaymentSchedule>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    }

    // ============================================================
    //  Ø¬Ø¯ÙˆÙ„ Ø§Ù„Ø¯ÙØ¹Ø§Øª Ø§Ù„Ù…Ø¬Ø¯ÙˆÙ„Ø© Ù„Ù„Ø¹Ù‚Ø¯ (PaymentSchedule)
    // ============================================================
    public class PaymentSchedule
    {
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }

        [Display(Name = "Ø§Ø³Ù… Ø§Ù„Ø¯ÙØ¹Ø© / Ø§Ù„Ù…Ø±Ø­Ù„Ø©")]
        [Required, StringLength(100)]
        public string StageName { get; set; } // Ù…Ø«Ø§Ù„: Ø¹Ø±Ø¨ÙˆÙ† ØªÙˆÙ‚ÙŠØ¹ Ø§Ù„Ø¹Ù‚Ø¯ (30%)ØŒ Ø¯ÙØ¹Ø© Ø¨Ø¯Ø¡ Ø§Ù„ØªØµÙ†ÙŠØ¹ (40%)...

        [Display(Name = "Ù†Ø³Ø¨Ø© Ø§Ù„Ø¯ÙØ¹Ø© (%)")]
        public decimal Percentage { get; set; }

        [Display(Name = "Ù‚ÙŠÙ…Ø© Ø§Ù„Ø¯ÙØ¹Ø© (Ø¯.Ù„)")]
        public decimal Amount { get; set; }

        [Display(Name = "ØªØ§Ø±ÙŠØ® Ø§Ù„Ø§Ø³ØªØ­Ù‚Ø§Ù‚ Ø§Ù„Ù…ØªÙˆÙ‚Ø¹")]
        public DateTime? DueDate { get; set; }

        [Display(Name = "Ø´Ø±Ø· Ø§Ø³ØªØ­Ù‚Ø§Ù‚ Ø§Ù„Ø¯ÙØ¹Ø©")]
        [StringLength(200)]
        public string Condition { get; set; }

        [Display(Name = "Ù‡Ù„ ØªÙ… Ø³Ø¯Ø§Ø¯ Ø§Ù„Ø¯ÙØ¹Ø©ØŸ")]
        public bool IsPaid { get; set; } = false;

        [Display(Name = "ØªØ§Ø±ÙŠØ® Ø§Ù„Ø³Ø¯Ø§Ø¯ Ø§Ù„ÙØ¹Ù„ÙŠ")]
        public DateTime? PaidAt { get; set; }

        [Display(Name = "Ù…Ø¹Ø±Ù Ø¥ÙŠØµØ§Ù„ Ø§Ù„Ù‚Ø¨Ø¶")]
        public int? CustomerReceiptId { get; set; }

        // Ø§Ù„Ø¹Ù„Ø§Ù‚Ø©
        public virtual Contract Contract { get; set; }
    }
}

