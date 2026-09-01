using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOSS.Web.Models
{
    // ============================================================
    //  ÙØ¦Ø§Øª Ø¨Ù†ÙˆØ¯ Ø¹Ø±Ø¶ Ø§Ù„Ø³Ø¹Ø±
    // ============================================================
    public enum QuotationItemCategory
    {
        [Display(Name = "Ø®Ø§Ù…Ø§Øª ÙˆØ£Ù„ÙˆØ§Ø­ Ø§Ù„Ø®Ø´Ø¨")]
        WoodMaterials = 1,

        [Display(Name = "Ù…ÙØµÙ„Ø§Øª ÙˆØ¥ÙƒØ³Ø³ÙˆØ§Ø±Ø§Øª ÙˆÙ…Ù‚Ø§Ø¨Ø¶")]
        HardwareAndAccessories = 2,

        [Display(Name = "Ø£Ø³Ø·Ø­ (Ø±Ø®Ø§Ù… / ÙƒÙˆØ§Ø±ØªØ² / Ø¬Ø±Ø§Ù†ÙŠØª)")]
        Countertops = 3,

        [Display(Name = "Ø£Ø¬Ù‡Ø²Ø© ÙƒÙ‡Ø±ÙˆÙ…Ù†Ø²Ù„ÙŠØ© ÙˆØ­ÙˆØ¶")]
        AppliancesAndSinks = 4,

        [Display(Name = "Ø£Ø¬ÙˆØ± Ø§Ù„Ù…ØµÙ†Ø¹ÙŠØ© ÙˆØ§Ù„ØªØµÙ†ÙŠØ¹")]
        ManufacturingLabor = 5,

        [Display(Name = "Ø®Ø¯Ù…Ø§Øª Ø§Ù„Ù†Ù‚Ù„ ÙˆØ§Ù„ØªØ±ÙƒÙŠØ¨")]
        InstallationAndDelivery = 6,

        [Display(Name = "Ø£Ø®Ø±Ù‰")]
        Other = 7
    }

    // ============================================================
    //  Ø¹Ø±ÙˆØ¶ Ø§Ù„Ø£Ø³Ø¹Ø§Ø± (Quotation) - Ø¯Ø¹Ù… Ø§Ù„Ø¥ØµØ¯Ø§Ø±Ø§Øª ÙˆØ§Ù„Ø§Ø¹ØªÙ…Ø§Ø¯
    // ============================================================
    public class Quotation
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ø·Ù„Ø¨ Ø§Ù„Ù…Ø·Ø¨Ø®")]
        public int KitchenRequestId { get; set; }

        [Display(Name = "Ø¥ØµØ¯Ø§Ø± Ø§Ù„ØªØµÙ…ÙŠÙ… Ø§Ù„Ù…Ø±ØªØ¨Ø·")]
        public int? DesignVersionId { get; set; }

        [Display(Name = "Ø±Ù‚Ù… Ø¹Ø±Ø¶ Ø§Ù„Ø³Ø¹Ø±")]
        [StringLength(50)]
        public string QuotationNumber { get; set; }

        [Display(Name = "Ø±Ù‚Ù… Ø§Ù„Ø¥ØµØ¯Ø§Ø±")]
        public int VersionNumber { get; set; } = 1;

        [Display(Name = "Ø§Ù„Ù…Ø¬Ù…ÙˆØ¹ Ù‚Ø¨Ù„ Ø§Ù„Ø®ØµÙ… (Ø¯.Ù„)")]
        public decimal SubTotal { get; set; }

        [Display(Name = "Ù‚ÙŠÙ…Ø© Ø§Ù„Ø®ØµÙ… (Ø¯.Ù„)")]
        public decimal Discount { get; set; } = 0;

        [Display(Name = "Ø§Ù„Ø¶Ø±ÙŠØ¨Ø© (Ø¥Ù† ÙˆØ¬Ø¯Øª) (Ø¯.Ù„)")]
        public decimal TaxAmount { get; set; } = 0;

        [Display(Name = "ØµØ§ÙÙŠ Ø§Ù„Ù‚ÙŠÙ…Ø© Ø§Ù„Ø¥Ø¬Ù…Ø§Ù„ÙŠØ© (Ø¯.Ù„)")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Ù…Ø¯Ø© ØµÙ„Ø§Ø­ÙŠØ© Ø§Ù„Ø¹Ø±Ø¶ (Ø£ÙŠØ§Ù…)")]
        public int ValidityDays { get; set; } = 15;

        [Display(Name = "ØªØ§Ø±ÙŠØ® Ø§Ù†ØªÙ‡Ø§Ø¡ Ø§Ù„ØµÙ„Ø§Ø­ÙŠØ©")]
        public DateTime ExpiryDate => CreatedAt.AddDays(ValidityDays);

        [Display(Name = "Ø´Ø±ÙˆØ· Ø§Ù„Ø¯ÙØ¹ Ø§Ù„Ù…ØªÙÙ‚ Ø¹Ù„ÙŠÙ‡Ø§")]
        [StringLength(500)]
        public string PaymentTerms { get; set; } = "30% Ø¹Ø±Ø¨ÙˆÙ† Ø¹Ù†Ø¯ Ø§Ù„ØªØ¹Ø§Ù‚Ø¯ØŒ 40% Ø¹Ù†Ø¯ Ø¨Ø¯Ø¡ Ø§Ù„ØªØµÙ†ÙŠØ¹ØŒ 20% Ø¹Ù†Ø¯ Ø§Ù„Ø¬Ø§Ù‡Ø²ÙŠØ© Ù„Ù„ØªØ±ÙƒÙŠØ¨ØŒ 10% Ø¹Ù†Ø¯ Ø§Ù„ØªØ³Ù„ÙŠÙ… Ø§Ù„Ù†Ù‡Ø§Ø¦ÙŠ.";

        [Display(Name = "Ø­Ø§Ù„Ø© Ø¹Ø±Ø¶ Ø§Ù„Ø³Ø¹Ø±")]
        public QuotationStatus Status { get; set; } = QuotationStatus.Draft;

        [Display(Name = "Ø§Ù„Ù…Ø¹ØªÙ…Ø¯ Ø¥Ø¯Ø§Ø±ÙŠØ§Ù‹")]
        public string ApprovedBy { get; set; }

        [Display(Name = "ØªØ§Ø±ÙŠØ® Ø§Ù„Ø§Ø¹ØªÙ…Ø§Ø¯ Ø§Ù„Ø¯Ø§Ø®Ù„ÙŠ")]
        public DateTime? ApprovedAt { get; set; }

        [Display(Name = "ØªØ§Ø±ÙŠØ® Ø§Ù„Ø¥Ø±Ø³Ø§Ù„ Ù„Ù„Ø¹Ù…ÙŠÙ„")]
        public DateTime? SentToCustomerAt { get; set; }

        [Display(Name = "ØªØ§Ø±ÙŠØ® Ù‚Ø¨ÙˆÙ„ Ø§Ù„Ø¹Ù…ÙŠÙ„")]
        public DateTime? AcceptedAt { get; set; }

        [Display(Name = "Ù…Ù„Ø§Ø­Ø¸Ø§Øª Ø¥Ø¶Ø§ÙÙŠØ©")]
        [StringLength(1000)]
        public string Notes { get; set; }

        [Display(Name = "ØªØ§Ø±ÙŠØ® Ø§Ù„Ø¥Ù†Ø´Ø§Ø¡")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ø£ÙÙ†Ø´Ø¦ Ø¨ÙˆØ§Ø³Ø·Ø©")]
        public string CreatedBy { get; set; }

        // Ø§Ù„Ø¹Ù„Ø§Ù‚Ø§Øª
        public virtual KitchenRequest KitchenRequest { get; set; }
        public virtual DesignVersion DesignVersion { get; set; }
        public virtual ICollection<QuotationItem> Items { get; set; } = new List<QuotationItem>();
    }

    // ============================================================
    //  Ø¨Ù†Ø¯ ÙÙŠ Ø¹Ø±Ø¶ Ø§Ù„Ø³Ø¹Ø± (QuotationItem)
    // ============================================================
    public class QuotationItem
    {
        public int Id { get; set; }

        [Required]
        public int QuotationId { get; set; }

        [Display(Name = "ÙØ¦Ø© Ø§Ù„Ø¨Ù†Ø¯")]
        public QuotationItemCategory Category { get; set; } = QuotationItemCategory.WoodMaterials;

        [Display(Name = "Ø§Ø³Ù… Ø§Ù„Ø¨Ù†Ø¯ / Ø§Ù„ØµÙ†Ù")]
        [Required, StringLength(200)]
        public string ItemName { get; set; }

        [Display(Name = "Ø§Ù„ÙˆØµÙ ÙˆØ§Ù„Ù…ÙˆØ§ØµÙØ§Øª Ø§Ù„ÙÙ†ÙŠØ©")]
        [StringLength(500)]
        public string Description { get; set; }

        [Display(Name = "Ø§Ù„ÙˆØ­Ø¯Ø©")]
        [StringLength(30)]
        public string Unit { get; set; } = "Ù…ØªØ±";

        [Display(Name = "Ø§Ù„ÙƒÙ…ÙŠØ©")]
        public decimal Quantity { get; set; } = 1;

        [Display(Name = "Ø³Ø¹Ø± Ø§Ù„ÙˆØ­Ø¯Ø© (Ø¯.Ù„)")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Ø®ØµÙ… Ø§Ù„Ø¨Ù†Ø¯ (Ø¯.Ù„)")]
        public decimal Discount { get; set; } = 0;

        [Display(Name = "Ø§Ù„Ø¥Ø¬Ù…Ø§Ù„ÙŠ (Ø¯.Ù„)")]
        public decimal TotalPrice { get; set; }

        // Ø§Ù„Ø¹Ù„Ø§Ù‚Ø©
        public virtual Quotation Quotation { get; set; }
    }
}

