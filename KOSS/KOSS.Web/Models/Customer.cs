using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOSS.Web.Models
{
    // ============================================================
    //  Ø§Ù„Ø¹Ù…ÙŠÙ„ (Customer)
    // ============================================================
    public class Customer
    {
        public int Id { get; set; }

        [Display(Name = "Ø§Ø³Ù… Ø§Ù„Ø¹Ù…ÙŠÙ„")]
        [Required(ErrorMessage = "Ø§Ø³Ù… Ø§Ù„Ø¹Ù…ÙŠÙ„ Ù…Ø·Ù„ÙˆØ¨"), StringLength(150)]
        public string Name { get; set; }

        [Display(Name = "Ø±Ù‚Ù… Ø§Ù„Ù‡Ø§ØªÙ Ø§Ù„Ø£Ø³Ø§Ø³ÙŠ")]
        [Required(ErrorMessage = "Ø±Ù‚Ù… Ø§Ù„Ù‡Ø§ØªÙ Ù…Ø·Ù„ÙˆØ¨"), StringLength(30)]
        public string Phone { get; set; }

        [Display(Name = "Ø±Ù‚Ù… Ù‡Ø§ØªÙ Ø¥Ø¶Ø§ÙÙŠ")]
        [StringLength(30)]
        public string Phone2 { get; set; }

        [Display(Name = "Ø§Ù„Ø¨Ø±ÙŠØ¯ Ø§Ù„Ø¥Ù„ÙƒØªØ±ÙˆÙ†ÙŠ")]
        [StringLength(100), EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Ø§Ù„Ù…Ø¯ÙŠÙ†Ø© / Ø§Ù„Ù…Ù†Ø·Ù‚Ø©")]
        [StringLength(100)]
        public string District { get; set; }

        [Display(Name = "Ø§Ù„Ø¹Ù†ÙˆØ§Ù† Ø§Ù„ØªÙØµÙŠÙ„ÙŠ")]
        [StringLength(300)]
        public string Address { get; set; }

        [Display(Name = "Ù…ØµØ¯Ø± Ø§Ù„Ù…Ø¹Ø±ÙØ© Ø¨Ø§Ù„Ø´Ø±ÙƒØ©")]
        [StringLength(100)]
        public string LeadSource { get; set; } // ÙÙŠØ³Ø¨ÙˆÙƒØŒ ØªÙˆØµÙŠØ© Ø¹Ù…ÙŠÙ„ØŒ Ù…Ø¹Ø±Ø¶ØŒ Ù„ÙˆØ­Ø© Ø¥Ø¹Ù„Ø§Ù†ÙŠØ©...

        [Display(Name = "Ø§Ù„Ø±Ù‚Ù… Ø§Ù„ÙˆØ·Ù†ÙŠ / Ø§Ù„Ø¶Ø±ÙŠØ¨ÙŠ")]
        [StringLength(50)]
        public string NationalOrTaxId { get; set; }

        [Display(Name = "Ù…Ù„Ø§Ø­Ø¸Ø§Øª Ø¹Ø§Ù…Ø©")]
        [StringLength(500)]
        public string Notes { get; set; }

        [Display(Name = "ØªØ§Ø±ÙŠØ® Ø§Ù„ØªØ³Ø¬ÙŠÙ„")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ø³ÙØ¬Ù„ Ø¨ÙˆØ§Ø³Ø·Ø©")]
        public string CreatedBy { get; set; }

        // Ø§Ù„Ø¹Ù„Ø§Ù‚Ø§Øª
        public virtual ICollection<CustomerInquiry> Inquiries { get; set; } = new List<CustomerInquiry>();
        public virtual ICollection<KitchenRequest> KitchenRequests { get; set; } = new List<KitchenRequest>();
    }

    // ============================================================
    //  Ø­Ø§Ù„Ø© Ø§Ù„Ø§Ø³ØªÙØ³Ø§Ø±
    // ============================================================
    public enum InquiryStatus
    {
        [Display(Name = "Ø¬Ø¯ÙŠØ¯")]
        New = 1,

        [Display(Name = "ØªÙ… Ø§Ù„ØªÙˆØ§ØµÙ„")]
        Contacted = 2,

        [Display(Name = "Ù…Ø¤Ù‡Ù„ Ù„Ø·Ù„Ø¨ Ø±Ø³Ù…ÙŠ")]
        Qualified = 3,

        [Display(Name = "ØªÙ… Ø§Ù„ØªØ­ÙˆÙŠÙ„ Ù„Ø·Ù„Ø¨ Ù…Ø·Ø¨Ø®")]
        ConvertedToRequest = 4,

        [Display(Name = "ØºÙŠØ± Ù…Ù‡ØªÙ… / Ù…ØºÙ„Ù‚")]
        NotInterested = 5,

        [Display(Name = "Ù…Ø¤Ø¬Ù„ Ù„Ù„Ù…Ø³ØªÙ‚Ø¨Ù„")]
        Postponed = 6
    }

    // ============================================================
    //  Ø§Ù„Ø§Ø³ØªÙØ³Ø§Ø± ÙˆØ§Ù„ÙØ±ØµØ© Ø§Ù„Ø¨ÙŠØ¹ÙŠØ© (Customer Inquiry / Lead)
    // ============================================================
    public class CustomerInquiry
    {
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Display(Name = "Ø§Ù„Ù…ÙˆÙ‚Ø¹ / Ø¹Ù†ÙˆØ§Ù† Ø§Ù„Ù…Ø·Ø¨Ø®")]
        [StringLength(200)]
        public string Location { get; set; }

        [Display(Name = "Ø§Ù„Ù…Ø³Ø§Ø­Ø© Ø§Ù„ØªÙ‚Ø±ÙŠØ¨ÙŠØ© (Ù…Â²)")]
        public decimal? EstimatedAreaM2 { get; set; }

        [Display(Name = "Ø§Ù„Ù…ÙŠØ²Ø§Ù†ÙŠØ© Ø§Ù„ØªÙ‚Ø±ÙŠØ¨ÙŠØ© (Ø¯.Ù„)")]
        public decimal? EstimatedBudget { get; set; }

        [Display(Name = "Ù†ÙˆØ¹ Ø§Ù„Ù…Ø·Ø¨Ø® Ø§Ù„Ù…ÙØ¶Ù„")]
        public KitchenLayoutType? PreferredLayout { get; set; }

        [Display(Name = "Ø§Ù„Ù…ÙˆØ¹Ø¯ Ø§Ù„Ù…Ù†Ø§Ø³Ø¨ Ù„Ù„ØªÙˆØ§ØµÙ„")]
        [StringLength(100)]
        public string PreferredContactTime { get; set; }

        [Display(Name = "Ø­Ø§Ù„Ø© Ø§Ù„Ø§Ø³ØªÙØ³Ø§Ø±")]
        public InquiryStatus Status { get; set; } = InquiryStatus.New;

        [Display(Name = "Ø³Ø¨Ø¨ Ø§Ù„Ø®Ø³Ø§Ø±Ø© (Ø¥Ù† Ù„Ù… ÙŠØªÙ… Ø§Ù„Ø¹Ù‚Ø¯)")]
        [StringLength(300)]
        public string LostReason { get; set; }

        [Display(Name = "Ø§Ù„Ù…Ù„Ø§Ø­Ø¸Ø§Øª ÙˆØªÙØ§ØµÙŠÙ„ Ø§Ù„Ø§Ø­ØªÙŠØ§Ø¬")]
        [StringLength(500)]
        public string Notes { get; set; }

        [Display(Name = "Ù…Ø¹Ø±Ù Ø§Ù„Ø·Ù„Ø¨ Ø§Ù„Ù…Ø±ØªØ¨Ø· (Ø¥Ù† ØªÙ… Ø§Ù„ØªØ­ÙˆÙŠÙ„)")]
        public int? ConvertedKitchenRequestId { get; set; }

        [Display(Name = "ØªØ§Ø±ÙŠØ® Ø§Ù„Ø§Ø³ØªÙØ³Ø§Ø±")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ø³ÙØ¬Ù„ Ø¨ÙˆØ§Ø³Ø·Ø©")]
        public string CreatedBy { get; set; }

        // Ø§Ù„Ø¹Ù„Ø§Ù‚Ø§Øª
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("ConvertedKitchenRequestId")]
        public virtual KitchenRequest ConvertedKitchenRequest { get; set; }
    }
}

