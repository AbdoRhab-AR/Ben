using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOSS.Web.Models
{
    // ============================================================
    //  العميل (Customer)
    // ============================================================
    public class Customer
    {
        public int Id { get; set; }

        [Display(Name = "اسم العميل")]
        [Required(ErrorMessage = "اسم العميل مطلوب"), StringLength(150)]
        public string Name { get; set; }

        [Display(Name = "رقم الهاتف الأساسي")]
        [Required(ErrorMessage = "رقم الهاتف مطلوب"), StringLength(30)]
        [Index("IX_Customer_Phone", IsUnique = true)]
        public string Phone { get; set; }

        [Display(Name = "رقم هاتف إضافي")]
        [StringLength(30)]
        public string Phone2 { get; set; }

        [Display(Name = "البريد الإلكتروني")]
        [StringLength(100), EmailAddress]
        public string Email { get; set; }

        [Display(Name = "المدينة / المنطقة")]
        [StringLength(100)]
        public string District { get; set; }

        [Display(Name = "العنوان التفصيلي")]
        [StringLength(300)]
        public string Address { get; set; }

        [Display(Name = "مصدر المعرفة بالشركة")]
        [StringLength(100)]
        public string LeadSource { get; set; } // فيسبوك، توصية عميل، معرض، لوحة إعلانية...

        [Display(Name = "الرقم الوطني / الضريبي")]
        [StringLength(50)]
        public string NationalOrTaxId { get; set; }

        [Display(Name = "ملاحظات عامة")]
        [StringLength(500)]
        public string Notes { get; set; }

        [Display(Name = "تاريخ التسجيل")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "سُجل بواسطة")]
        public string CreatedBy { get; set; }

        // العلاقات
        public virtual ICollection<CustomerInquiry> Inquiries { get; set; } = new List<CustomerInquiry>();
        public virtual ICollection<KitchenRequest> KitchenRequests { get; set; } = new List<KitchenRequest>();
    }

    // ============================================================
    //  حالة الاستفسار
    // ============================================================
    public enum InquiryStatus
    {
        [Display(Name = "جديد")]
        New = 1,

        [Display(Name = "تم التواصل")]
        Contacted = 2,

        [Display(Name = "مؤهل لطلب رسمي")]
        Qualified = 3,

        [Display(Name = "تم التحويل لطلب مطبخ")]
        ConvertedToRequest = 4,

        [Display(Name = "غير مهتم / مغلق")]
        NotInterested = 5,

        [Display(Name = "مؤجل للمستقبل")]
        Postponed = 6
    }

    // ============================================================
    //  الاستفسار والفرصة البيعية (Customer Inquiry / Lead)
    // ============================================================
    public class CustomerInquiry
    {
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Display(Name = "الموقع / عنوان المطبخ")]
        [StringLength(200)]
        public string Location { get; set; }

        [Display(Name = "المساحة التقريبية (م²)")]
        public decimal? EstimatedAreaM2 { get; set; }

        [Display(Name = "الميزانية التقريبية (د.ل)")]
        public decimal? EstimatedBudget { get; set; }

        [Display(Name = "نوع المطبخ المفضل")]
        public KitchenLayoutType? PreferredLayout { get; set; }

        [Display(Name = "الموعد المناسب للتواصل")]
        [StringLength(100)]
        public string PreferredContactTime { get; set; }

        [Display(Name = "حالة الاستفسار")]
        public InquiryStatus Status { get; set; } = InquiryStatus.New;

        [Display(Name = "سبب الخسارة (إن لم يتم العقد)")]
        [StringLength(300)]
        public string LostReason { get; set; }

        [Display(Name = "الملاحظات وتفاصيل الاحتياج")]
        [StringLength(500)]
        public string Notes { get; set; }

        [Display(Name = "معرف الطلب المرتبط (إن تم التحويل)")]
        public int? ConvertedKitchenRequestId { get; set; }

        [Display(Name = "تاريخ الاستفسار")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "سُجل بواسطة")]
        public string CreatedBy { get; set; }

        // العلاقات
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("ConvertedKitchenRequestId")]
        public virtual KitchenRequest ConvertedKitchenRequest { get; set; }
    }
}
