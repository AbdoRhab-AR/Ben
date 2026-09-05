using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOSS.Web.Models
{
    // ============================================================
    //  فئات بنود عرض السعر
    // ============================================================
    public enum QuotationItemCategory
    {
        [Display(Name = "خامات وألواح الخشب")]
        WoodMaterials = 1,

        [Display(Name = "مفصلات وإكسسوارات ومقابض")]
        HardwareAndAccessories = 2,

        [Display(Name = "أسطح (رخام / كوارتز / جرانيت)")]
        Countertops = 3,

        [Display(Name = "أجهزة كهرومنزلية وحوض")]
        AppliancesAndSinks = 4,

        [Display(Name = "أجور المصنعية والتصنيع")]
        ManufacturingLabor = 5,

        [Display(Name = "خدمات النقل والتركيب")]
        InstallationAndDelivery = 6,

        [Display(Name = "أخرى")]
        Other = 7
    }

    // ============================================================
    //  حالة عرض السعر
    // ============================================================
    public enum QuotationStatus
    {
        [Display(Name = "مسودة تسعير")]
        Draft = 1,

        [Display(Name = "معتمد داخلياً")]
        InternalApproved = 2,

        [Display(Name = "مُرسل للعميل")]
        SentToCustomer = 3,

        [Display(Name = "مقبول من العميل")]
        Accepted = 4,

        [Display(Name = "مرفوض من العميل")]
        Rejected = 5,

        [Display(Name = "مُعدّل بإصدار جديد")]
        Revised = 6
    }

    // ============================================================
    //  طريقة التسعير بالسوق الليبي
    // ============================================================
    public enum PricingMethod
    {
        [Display(Name = "بالمتر الطولي (Running Meter)")]
        RunningMeter = 1,

        [Display(Name = "بالمتر المربع (Square Meter)")]
        SquareMeter = 2,

        [Display(Name = "تسعير تجميعي بالعلبة (Modular Box Pricing)")]
        ModularBoxPricing = 3
    }

    // ============================================================
    //  عروض الأسعار (Quotation)
    // ============================================================
    public class Quotation
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "طلب المطبخ")]
        public int KitchenRequestId { get; set; }

        [Display(Name = "إصدار التصميم المرتبط")]
        public int? DesignVersionId { get; set; }

        [Display(Name = "رقم عرض السعر")]
        [StringLength(50)]
        public string QuotationNumber { get; set; }

        [Display(Name = "رقم الإصدار")]
        public int VersionNumber { get; set; } = 1;

        [Display(Name = "المجموع قبل الخصم (د.ل)")]
        public decimal SubTotal { get; set; }

        [Display(Name = "قيمة الخصم (د.ل)")]
        public decimal Discount { get; set; } = 0;

        [Display(Name = "الضريبة (د.ل)")]
        public decimal TaxAmount { get; set; } = 0;

        [Display(Name = "صافي القيمة الإجمالية (د.ل)")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "مدة صلاحية العرض (أيام)")]
        public int ValidityDays { get; set; } = 15;

        [Display(Name = "تاريخ انتهاء الصلاحية")]
        public DateTime ExpiryDate => CreatedAt.AddDays(ValidityDays);

        [Display(Name = "شروط الدفع المتفق عليها")]
        [StringLength(500)]
        public string PaymentTerms { get; set; } = "30% عربون عند التعاقد، 40% عند بدء التصنيع، 20% عند الجاهزية للتركيب، 10% عند التسليم النهائي.";

        [Display(Name = "طريقة التسعير المعتمدة")]
        public PricingMethod Method { get; set; } = PricingMethod.RunningMeter;

        [Display(Name = "ملاحظات وتفصيل فروقات الأسعار")]
        [StringLength(1000)]
        public string PriceVarianceNotes { get; set; }

        [Display(Name = "حالة عرض السعر")]
        public QuotationStatus Status { get; set; } = QuotationStatus.Draft;

        [Display(Name = "المعتمد إدارياً")]
        public string ApprovedBy { get; set; }

        [Display(Name = "تاريخ الاعتماد الداخلي")]
        public DateTime? ApprovedAt { get; set; }

        [Display(Name = "ملاحظات إضافية على العرض")]
        [StringLength(1000)]
        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }

        // العلاقات
        public virtual KitchenRequest KitchenRequest { get; set; }
        public virtual DesignVersion DesignVersion { get; set; }
        public virtual ICollection<QuotationItem> Items { get; set; } = new List<QuotationItem>();
    }

    // ============================================================
    //  بند في عرض السعر (QuotationItem)
    // ============================================================
    public class QuotationItem
    {
        public int Id { get; set; }

        [Required]
        public int QuotationId { get; set; }

        [Display(Name = "الفئة")]
        public QuotationItemCategory Category { get; set; } = QuotationItemCategory.WoodMaterials;

        [Display(Name = "اسم البند / المواصفة")]
        [Required, StringLength(200)]
        public string ItemName { get; set; }

        [Display(Name = "الكمية")]
        public decimal Quantity { get; set; } = 1;

        [Display(Name = "الوحدة")]
        [StringLength(30)]
        public string Unit { get; set; } = "متر طولي";

        [Display(Name = "سعر الوحدة (د.ل)")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "الخصم على البند (د.ل)")]
        public decimal Discount { get; set; } = 0;

        [Display(Name = "الإجمالي (د.ل)")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "ملاحظات البند")]
        [StringLength(250)]
        public string Notes { get; set; }

        // العلاقة
        public virtual Quotation Quotation { get; set; }
    }
}
