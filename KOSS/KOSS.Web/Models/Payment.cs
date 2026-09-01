using System;
using System.ComponentModel.DataAnnotations;

namespace KOSS.Web.Models
{
    // ============================================================
    //  نوع الدفعة
    // ============================================================
    public enum PaymentType
    {
        [Display(Name = "عربون / دفعة مقدمة")]
        Deposit = 1,

        [Display(Name = "دفعة دورية")]
        Installment = 2,

        [Display(Name = "الدفعة النهائية (30%)")]
        FinalPayment = 3,

        [Display(Name = "رسوم تصميم")]
        DesignFee = 4
    }

    // ============================================================
    //  طريقة الدفع
    // ============================================================
    public enum PaymentMethod
    {
        [Display(Name = "نقداً")]
        Cash = 1,

        [Display(Name = "تحويل بنكي")]
        BankTransfer = 2,

        [Display(Name = "شيك")]
        Cheque = 3
    }

    // ============================================================
    //  نموذج الدفعة
    // ============================================================
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }

        [Display(Name = "رقم الإيصال (تسلسلي)")]
        public string ReceiptNumber { get; set; }

        [Required]
        [Display(Name = "المبلغ المدفوع (د.ل)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal Amount { get; set; }

        [Display(Name = "نوع الدفعة")]
        public PaymentType PaymentType { get; set; } = PaymentType.Deposit;

        [Display(Name = "طريقة الدفع")]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

        [Display(Name = "رقم المرجع / الشيك")]
        [StringLength(100)]
        public string ReferenceNumber { get; set; }

        [Display(Name = "تاريخ الدفع")]
        public DateTime PaidAt { get; set; } = DateTime.Now;

        [Display(Name = "استُلم بواسطة")]
        public string ReceivedBy { get; set; }

        [Display(Name = "ملاحظات")]
        [StringLength(300)]
        public string Notes { get; set; }

        // العلاقة
        public virtual Contract Contract { get; set; }
    }
}
