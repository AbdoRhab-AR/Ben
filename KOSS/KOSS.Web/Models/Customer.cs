using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOSS.Web.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Display(Name = "اسم العميل")]
        [Required(ErrorMessage = "اسم العميل مطلوب"), StringLength(150)]
        public string Name { get; set; }

        [Display(Name = "رقم الهاتف")]
        [Required(ErrorMessage = "رقم الهاتف مطلوب"), StringLength(30)]
        public string Phone { get; set; }

        [Display(Name = "المدينة / المنطقة")]
        [StringLength(100)]
        public string District { get; set; }

        [Display(Name = "العنوان التفصيلي")]
        [StringLength(250)]
        public string Address { get; set; }

        [Display(Name = "ملاحظات عامة حول العميل")]
        [StringLength(500)]
        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }

        public virtual ICollection<CustomerInquiry> Inquiries { get; set; } = new List<CustomerInquiry>();
        public virtual ICollection<KitchenRequest> KitchenRequests { get; set; } = new List<KitchenRequest>();
    }

    public enum InquiryStatus
    {
        [Display(Name = "استفسار جديد")]
        New = 1,

        [Display(Name = "تم التواصل والتأكيد")]
        Contacted = 2,

        [Display(Name = "تحول إلى طلب مطبخ رسمي")]
        ConvertedToRequest = 3,

        [Display(Name = "مغلق / غير مهتم")]
        ClosedNotInterested = 4
    }

    public class CustomerInquiry
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "العميل")]
        public int CustomerId { get; set; }

        [Display(Name = "المدينة / المنطقة")]
        [StringLength(100)]
        public string Location { get; set; }

        [Display(Name = "المساحة التقريبية (م²)")]
        public decimal? EstimatedAreaM2 { get; set; }

        [Display(Name = "الميزانية المتوقعة (د.ل)")]
        public decimal? EstimatedBudget { get; set; }

        [Display(Name = "التخطيط المفضل")]
        public KitchenLayoutType? PreferredLayout { get; set; }

        [Display(Name = "ملاحظات الاستفسار واحتياجات العميل")]
        [StringLength(1000)]
        public string Notes { get; set; }

        [Display(Name = "حالة الاستفسار")]
        public InquiryStatus Status { get; set; } = InquiryStatus.New;

        [Display(Name = "طلب المطبخ الناتج")]
        public int? ConvertedKitchenRequestId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual KitchenRequest ConvertedKitchenRequest { get; set; }
    }
}
