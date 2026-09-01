using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace KOSS.Web.Models
{
    // ============================================================
    //  Ø·Ù„Ø¨ Ø§Ù„Ù…Ø·Ø¨Ø® / Ø§Ù„Ù…Ø´Ø±ÙˆØ¹ Ø§Ù„Ù…Ø±ÙƒØ²ÙŠ (KitchenRequest / Project Core)
    // ============================================================
    public class KitchenRequest
    {
        public int Id { get; set; }

        [Display(Name = "Ø±Ù‚Ù… Ø§Ù„Ø·Ù„Ø¨ Ø§Ù„Ø±Ø³Ù…ÙŠ")]
        [StringLength(50)]
        public string RequestNumber { get; set; }

        [Required]
        [Display(Name = "Ø§Ù„Ø¹Ù…ÙŠÙ„")]
        public int CustomerId { get; set; }

        [Display(Name = "Ø¹Ù†ÙˆØ§Ù† ÙˆÙ…ÙˆÙ‚Ø¹ Ø§Ù„Ù…Ø·Ø¨Ø®")]
        [Required(ErrorMessage = "Ù…ÙˆÙ‚Ø¹ Ø§Ù„Ù…Ø·Ø¨Ø® Ù…Ø·Ù„ÙˆØ¨"), StringLength(250)]
        public string Location { get; set; }

        [Display(Name = "Ù†ÙˆØ¹ Ø§Ù„Ù…Ø´Ø±ÙˆØ¹")]
        public ProjectType ProjectType { get; set; } = ProjectType.Villa;

        [Display(Name = "ØªØ®Ø·ÙŠØ· Ø§Ù„Ù…Ø·Ø¨Ø®")]
        public KitchenLayoutType LayoutType { get; set; } = KitchenLayoutType.Straight;

        [Display(Name = "Ù…ÙˆØ¸Ù Ø§Ù„Ù…Ø¨ÙŠØ¹Ø§Øª Ø§Ù„Ù…Ø³Ø¤ÙˆÙ„")]
        public int? AssignedSalesStaffId { get; set; }

        [Display(Name = "Ø§Ù„Ù…ÙˆØ¹Ø¯ Ø§Ù„Ù…ØªÙˆÙ‚Ø¹ Ù„Ù„ØªØ³Ù„ÙŠÙ…")]
        public DateTime? TargetDeliveryDate { get; set; }

        [Display(Name = "Ø§Ù„Ø­Ø§Ù„Ø© Ø§Ù„Ø±Ø§Ù‡Ù†Ø©")]
        public KitchenRequestStatus Status { get; set; } = KitchenRequestStatus.RequestOpened;

        [Display(Name = "Ø§Ù„Ù…Ù„Ø§Ø­Ø¸Ø§Øª ÙˆØ§Ù„Ø§Ø­ØªÙŠØ§Ø¬Ø§Øª Ø§Ù„Ø£ÙˆÙ„ÙŠØ©")]
        [StringLength(1000)]
        public string Notes { get; set; }

        [Display(Name = "Ø³Ø¨Ø¨ Ø§Ù„Ø¥Ù„ØºØ§Ø¡ / Ø§Ù„Ø±ÙØ¶ (Ø¥Ù† ÙˆØ¬Ø¯)")]
        [StringLength(300)]
        public string CancellationReason { get; set; }

        [Display(Name = "ØªØ§Ø±ÙŠØ® Ø§Ù„Ø¥Ù†Ø´Ø§Ø¡")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "ØªØ§Ø±ÙŠØ® Ø¢Ø®Ø± ØªØ­Ø¯ÙŠØ«")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ø£ÙÙ†Ø´Ø¦ Ø¨ÙˆØ§Ø³Ø·Ø©")]
        public string CreatedBy { get; set; }

        // ============================================================
        //  Ø®ØµØ§Ø¦Øµ Ù…Ø­Ø³ÙˆØ¨Ø© Ù„Ù„Ø±Ø¨Ø· Ø§Ù„Ø³Ø±ÙŠØ¹
        // ============================================================
        [NotMapped]
        public Contract ActiveContract => Contracts != null ? Contracts.FirstOrDefault(c => c.Status == ContractStatus.Active || c.Status == ContractStatus.Completed) : null;

        [NotMapped]
        public DesignVersion ApprovedDesign => DesignVersions != null ? DesignVersions.FirstOrDefault(d => d.Status == DesignVersionStatus.ApprovedByCustomer) : null;

        [NotMapped]
        public Quotation AcceptedQuotation => Quotations != null ? Quotations.FirstOrDefault(q => q.Status == QuotationStatus.Accepted) : null;

        [NotMapped]
        public SiteVisit ApprovedSiteVisit => SiteVisits != null ? SiteVisits.FirstOrDefault(s => s.Status == SiteVisitStatus.Approved) : null;

        [NotMapped]
        public WorkOrder CurrentWorkOrder => WorkOrders != null ? WorkOrders.FirstOrDefault(w => w.Status != WorkOrderStatus.Cancelled) : null;

        // ============================================================
        //  Ø§Ù„Ø¹Ù„Ø§Ù‚Ø§Øª Ø§Ù„Ù…ØªÙØ±Ø¹Ø© Ø¹Ù† Ø·Ù„Ø¨ Ø§Ù„Ù…Ø·Ø¨Ø®
        // ============================================================
        public virtual Customer Customer { get; set; }
        public virtual StaffMember AssignedSalesStaff { get; set; }

        public virtual ICollection<RequestStatusHistory> StatusHistories { get; set; } = new List<RequestStatusHistory>();
        public virtual ICollection<SiteVisit> SiteVisits { get; set; } = new List<SiteVisit>();
        public virtual ICollection<DesignVersion> DesignVersions { get; set; } = new List<DesignVersion>();
        public virtual ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();
        public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
        public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
        public virtual ICollection<ProjectExpense> Expenses { get; set; } = new List<ProjectExpense>();
    }

    // ============================================================
    //  Ø³Ø¬Ù„ Ø§Ù„Ø­Ø§Ù„Ø§Øª Ø§Ù„ØªØ§Ø±ÙŠØ®ÙŠ Ù„Ù„Ø·Ù„Ø¨ (RequestStatusHistory)
    // ============================================================
    public class RequestStatusHistory
    {
        public int Id { get; set; }

        [Required]
        public int KitchenRequestId { get; set; }

        [Display(Name = "Ø§Ù„Ø­Ø§Ù„Ø© Ø§Ù„Ø³Ø§Ø¨Ù‚Ø©")]
        public KitchenRequestStatus OldStatus { get; set; }

        [Display(Name = "Ø§Ù„Ø­Ø§Ù„Ø© Ø§Ù„Ø¬Ø¯ÙŠØ¯Ø©")]
        public KitchenRequestStatus NewStatus { get; set; }

        [Display(Name = "Ø³Ø¨Ø¨ Ø§Ù„Ø§Ù†ØªÙ‚Ø§Ù„ / Ø§Ù„Ù…Ù„Ø§Ø­Ø¸Ø§Øª")]
        [StringLength(500)]
        public string Notes { get; set; }

        [Display(Name = "ØªØ§Ø±ÙŠØ® ÙˆÙˆÙ‚Øª Ø§Ù„ØªØºÙŠÙŠØ±")]
        public DateTime ChangedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ø§Ù„Ù…Ø³ØªØ®Ø¯Ù… Ø§Ù„Ø°ÙŠ Ù†ÙØ° Ø§Ù„ØªØºÙŠÙŠØ±")]
        public string ChangedBy { get; set; }

        // Ø§Ù„Ø¹Ù„Ø§Ù‚Ø©
        public virtual KitchenRequest KitchenRequest { get; set; }
    }
}

