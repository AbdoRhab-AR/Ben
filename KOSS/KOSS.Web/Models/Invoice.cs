using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KOSS.Web.Models
{
    // ============================================================
    //  الفاتورة الرئيسية للعقد
    // ============================================================
    public class Invoice
    {
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }

        [Display(Name = "رقم الفاتورة")]
        public string InvoiceNumber { get; set; }

        [Display(Name = "سعر المتر المعتمد (د.ل)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal PricePerMeter { get; set; }

        [Display(Name = "إجمالي الفاتورة (د.ل)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "الخصم (د.ل)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal Discount { get; set; } = 0;

        [Display(Name = "رسوم التصميم المخصومة (د.ل)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal DesignFeeDeduction { get; set; } = 0;

        [Display(Name = "الصافي بعد الخصم (د.ل)")]
        public decimal NetAmount => TotalAmount - Discount - DesignFeeDeduction;

        [Display(Name = "تاريخ الفاتورة")]
        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        [Display(Name = "حُرِّرت بواسطة")]
        public string CreatedBy { get; set; }

        [Display(Name = "ملاحظات")]
        [StringLength(500)]
        public string Notes { get; set; }

        // العلاقات
        public virtual Contract Contract { get; set; }
        public virtual ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    }

    // ============================================================
    //  بند من بنود الفاتورة
    // ============================================================
    public class InvoiceItem
    {
        public int Id { get; set; }

        [Required]
        public int InvoiceId { get; set; }

        [Display(Name = "اسم البند")]
        [StringLength(200)]
        public string ItemName { get; set; }

        [Display(Name = "الوحدة")]
        [StringLength(30)]
        public string Unit { get; set; } = "متر";

        [Display(Name = "الكمية")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Quantity { get; set; }

        [Display(Name = "سعر الوحدة (د.ل)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "الإجمالي (د.ل)")]
        public decimal Total => Quantity * UnitPrice;

        // العلاقة
        public virtual Invoice Invoice { get; set; }
    }
}
