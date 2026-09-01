using System;
using System.ComponentModel.DataAnnotations;

namespace KOSS.Web.Models
{
    // ============================================================
    //  رسوم التصميم
    // ============================================================
    public class DesignFee
    {
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }

        [Display(Name = "عدد الوحدات المصممة")]
        public int UnitCount { get; set; }

        [Display(Name = "المبلغ المدفوع (د.ل)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal FeeAmount { get; set; }

        [Display(Name = "هل تم الدفع؟")]
        public bool IsPaid { get; set; } = false;

        [Display(Name = "رقم الإيصال")]
        public string ReceiptNumber { get; set; }

        [Display(Name = "تاريخ الدفع")]
        public DateTime? PaidAt { get; set; }

        [Display(Name = "تم الخصم من الفاتورة النهائية؟")]
        public bool DeductedFromFinalInvoice { get; set; } = false;

        [Display(Name = "استُلم بواسطة")]
        public string ReceivedBy { get; set; }

        [Display(Name = "ملاحظات")]
        [StringLength(300)]
        public string Notes { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // العلاقة
        public virtual Contract Contract { get; set; }

        // ============================================================
        //  حساب رسوم التصميم تلقائياً حسب عدد الوحدات
        // ============================================================
        public static decimal CalculateFee(int unitCount)
        {
            if (unitCount == 1) return 300m;
            if (unitCount == 2) return 600m;
            if (unitCount <= 4) return 1200m;  // خصم على 4 وحدات
            // 5 وحدات فأكثر: تسعير مفتوح (يتطلب موافقة المدير)
            return unitCount * 300m;
        }

        public static bool RequiresManagerApproval(int unitCount) => unitCount >= 5;
    }
}
