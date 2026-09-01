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
        [Display(Name = "جديد")]
        New = 1,

        [Display(Name = "تمت المعاينة")]
        Measured = 2,

        [Display(Name = "تم دفع رسوم التصميم")]
        FeePaid = 3,

        [Display(Name = "تم التصميم")]
        Designed = 4,

        [Display(Name = "تم دفع العربون")]
        DepositPaid = 5,

        [Display(Name = "قيد التصنيع")]
        UnderProduction = 6,

        [Display(Name = "تم التصنيع")]
        Manufactured = 7,

        [Display(Name = "تم التركيب")]
        Installed = 8,

        [Display(Name = "قيد التسليم")]
        Commissioning = 9,

        [Display(Name = "مكتمل")]
        Completed = 10,

        [Display(Name = "ملغى")]
        Cancelled = 11,

        [Display(Name = "مسودة عقد")]
        Draft = 12,

        [Display(Name = "بانتظار سداد العربون")]
        AwaitingDeposit = 13,

        [Display(Name = "عقد نشط وسارٍ")]
        Active = 14,

        [Display(Name = "معلق / موقوف")]
        Suspended = 15,

        [Display(Name = "ملحق معدل")]
        Amended = 16,

        [Display(Name = "ملغى / منسوخ")]
        Terminated = 17
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

        [Display(Name = "العميل")]
        public int? ClientId { get; set; }

        [Display(Name = "عرض السعر المعتمد")]
        public int? QuotationId { get; set; }

        [Display(Name = "إصدار التصميم المعتمد")]
        public int? DesignVersionId { get; set; }

        [Display(Name = "رقم العقد")]
        [StringLength(50)]
        [Index("IX_Contract_Number", IsUnique = true)]
        public string ContractNumber { get; set; }

        [Display(Name = "القيمة الإجمالية للعقد (د.ل)")]
        public decimal TotalValue { get; set; }

        [Display(Name = "العربون المطلوب للتفعيل (د.ل)")]
        public decimal RequiredDeposit { get; set; }

        [Display(Name = "إجمالي المقبوضات (د.ل)")]
        public decimal TotalPaid { get; set; } = 0;

        [Display(Name = "المتبقي للتحصيل (د.ل)")]
        public decimal RemainingBalance => Math.Max(0, TotalValue - TotalPaid);

        [Display(Name = "نسبة السداد (%)")]
        public decimal PaymentPercentage => TotalValue > 0 ? (TotalPaid / TotalValue) * 100 : 0;

        [Display(Name = "سعر المتر المعتمد (د.ل)")]
        public decimal PricePerMeter { get; set; }

        [Display(Name = "إجمالي الأمتار المعتمدة")]
        public decimal TotalMeters { get; set; }

        [Display(Name = "تاريخ توقيع العقد")]
        public DateTime? SignedDate { get; set; }

        [Display(Name = "تاريخ التسليم المتفق عليه")]
        public DateTime? TargetCompletionDate { get; set; }

        [Display(Name = "الشرط الجزائي عن كل يوم تأخير (د.ل)")]
        public decimal PenaltyPerDay { get; set; } = 0;

        [Display(Name = "رابط ملف العقد الموقع الممسوح ضوئياً")]
        [StringLength(500)]
        public string SignedContractFilePath { get; set; }

        [Display(Name = "حالة العقد")]
        public ContractStatus Status { get; set; } = ContractStatus.Draft;

        [Display(Name = "شروط وملاحظات العقد")]
        [StringLength(1000)]
        public string Notes { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "تاريخ آخر تعديل")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Display(Name = "أُنشئ بواسطة")]
        public string CreatedBy { get; set; }

        // العلاقات
        public virtual KitchenRequest KitchenRequest { get; set; }
        public virtual Client Client { get; set; }
        public virtual Quotation Quotation { get; set; }
        public virtual DesignVersion DesignVersion { get; set; }

        public virtual ICollection<PaymentSchedule> PaymentSchedules { get; set; } = new List<PaymentSchedule>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
        public virtual ICollection<DesignFee> DesignFees { get; set; } = new List<DesignFee>();
        public virtual ICollection<KitchenUnit> Units { get; set; } = new List<KitchenUnit>();
        public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();

        [NotMapped]
        public DesignFee DesignFee => DesignFees?.FirstOrDefault();
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
        public string StageName { get; set; } // مثال: عربون توقيع العقد (30%)، دفعة بدء التصنيع (40%)...

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

        [Display(Name = "معرف إيصال القبض")]
        public int? CustomerReceiptId { get; set; }

        // العلاقة
        public virtual Contract Contract { get; set; }
    }
}
