using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace KOSS.Web.Models
{
    // ============================================================
    //  حالة العقد
    // ============================================================
    public enum ContractStatus
    {
        [Display(Name = "مسودة عقد")]
        Draft = 1,

        [Display(Name = "بانتظار سداد العربون")]
        AwaitingDeposit = 2,

        [Display(Name = "عقد نشط وسارٍ")]
        Active = 3,

        [Display(Name = "معلق / موقوف")]
        Suspended = 4,

        [Display(Name = "ملحق معدل")]
        Amended = 5,

        [Display(Name = "مكتمل نهائياً")]
        Completed = 6,

        [Display(Name = "ملغى / مفسوخ")]
        Cancelled = 7
    }

    // ============================================================
    //  العقد الرسمي (Contract)
    // ============================================================
    public class Contract
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "طلب المطبخ")]
        public int KitchenRequestId { get; set; }

        [Display(Name = "عرض السعر المعتمد")]
        public int? QuotationId { get; set; }

        [Display(Name = "إصدار التصميم المعتمد")]
        public int? DesignVersionId { get; set; }

        [Display(Name = "رقم العقد الرسمي")]
        [Required, StringLength(50)]
        public string ContractNumber { get; set; }

        [Display(Name = "تاريخ التوقيع والتعاقد")]
        public DateTime SignedDate { get; set; } = DateTime.Now;

        [Display(Name = "تاريخ التسليم والتركيب المستهدف")]
        public DateTime? TargetCompletionDate { get; set; }

        [Display(Name = "إجمالي قيمة العقد (د.ل)")]
        [Required]
        public decimal TotalValue { get; set; }

        [Display(Name = "قيمة العربون المشترط لتفعيل العقد (د.ل)")]
        public decimal RequiredDeposit { get; set; }

        [Display(Name = "إجمالي المبالغ المسددة فعلياً (د.ل)")]
        public decimal TotalPaid { get; set; } = 0;

        [Display(Name = "المبلغ المتبقي للتحصيل (د.ل)")]
        public decimal RemainingBalance => Math.Max(0, TotalValue - TotalPaid);

        [Display(Name = "سعر المتر الطولي التعاقدي (د.ل)")]
        public decimal PricePerMeter { get; set; }

        [Display(Name = "إجمالي الأمتار الطولية للمطبخ")]
        public decimal TotalMeters { get; set; }

        [Display(Name = "حالة العقد")]
        public ContractStatus Status { get; set; } = ContractStatus.Draft;

        [Display(Name = "شروط وأحكام خاصة بالعقد")]
        [StringLength(1000)]
        public string SpecialTerms { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }

        // العلاقات
        public virtual KitchenRequest KitchenRequest { get; set; }
        public virtual Client Client { get; set; }
        public virtual Quotation Quotation { get; set; }
        public virtual DesignVersion DesignVersion { get; set; }

        public virtual ICollection<PaymentSchedule> PaymentSchedules { get; set; } = new List<PaymentSchedule>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    }

    // ============================================================
    //  جدول الدفعات المجدولة للعقد (PaymentSchedule)
    // ============================================================
    public class PaymentSchedule
    {
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }

        [Display(Name = "اسم الدفعة / المرحلة")]
        [Required, StringLength(100)]
        public string StageName { get; set; }

        [Display(Name = "نسبة الدفعة (%)")]
        public decimal Percentage { get; set; }

        [Display(Name = "قيمة الدفعة (د.ل)")]
        public decimal Amount { get; set; }

        [Display(Name = "تاريخ الاستحقاق المتوقع")]
        public DateTime? DueDate { get; set; }

        [Display(Name = "شرط استحقاق الدفعة")]
        [StringLength(200)]
        public string Condition { get; set; }

        [Display(Name = "هل تم سداد الدفعة؟")]
        public bool IsPaid { get; set; } = false;

        [Display(Name = "تاريخ السداد الفعلي")]
        public DateTime? PaidAt { get; set; }

        // العلاقة
        public virtual Contract Contract { get; set; }
    }
}
