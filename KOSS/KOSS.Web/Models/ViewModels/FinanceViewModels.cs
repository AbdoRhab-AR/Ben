using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KOSS.Web.Models;

namespace KOSS.Web.Models.ViewModels
{
    // ============================================================
    //  ViewModel تسجيل دفعة مالية
    // ============================================================
    public class RecordPaymentViewModel
    {
        public int ContractId { get; set; }

        [Display(Name = "اسم العميل")]
        public string ClientName { get; set; }

        [Display(Name = "رقم العقد")]
        public string ContractNumber { get; set; }

        [Display(Name = "إجمالي قيمة العقد (د.ل)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal TotalValue { get; set; }

        [Display(Name = "إجمالي المدفوع (د.ل)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal TotalPaid { get; set; }

        [Display(Name = "المتبقي (د.ل)")]
        public decimal Remaining => TotalValue - TotalPaid;

        [Required(ErrorMessage = "المبلغ مطلوب")]
        [Display(Name = "المبلغ المدفوع (د.ل)")]
        [Range(0.001, 999999999, ErrorMessage = "يجب أن يكون المبلغ أكبر من صفر")]
        public decimal Amount { get; set; }

        [Display(Name = "نوع الدفعة")]
        public PaymentType PaymentType { get; set; } = PaymentType.Deposit;

        [Display(Name = "طريقة الدفع")]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

        [Display(Name = "رقم المرجع / الشيك")]
        [StringLength(100)]
        public string ReferenceNumber { get; set; }

        [Display(Name = "ملاحظات")]
        [StringLength(300)]
        public string Notes { get; set; }

        // نتيجة التخصيص التلقائي للعربون
        public List<UnitAllocationResult> AllocationResults { get; set; } = new List<UnitAllocationResult>();
    }

    // ============================================================
    //  نتيجة تخصيص العربون لكل وحدة
    // ============================================================
    public class UnitAllocationResult
    {
        public string UnitName          { get; set; }
        public decimal EstimatedValue   { get; set; }
        public decimal Required70Pct    { get; set; }
        public decimal Allocated        { get; set; }
        public string  Status           { get; set; }  // نشط / موقوف
    }

    // ============================================================
    //  ViewModel تحديث سعر المتر
    // ============================================================
    public class UpdatePriceViewModel
    {
        [Required(ErrorMessage = "سعر المتر مطلوب")]
        [Display(Name = "سعر المتر الجديد (د.ل)")]
        [Range(0.001, 999999, ErrorMessage = "السعر يجب أن يكون أكبر من صفر")]
        public decimal NewPricePerMeter { get; set; }

        [Display(Name = "السعر الحالي (د.ل)")]
        public decimal CurrentPrice { get; set; }

        [Display(Name = "ملاحظات سبب التغيير")]
        [StringLength(300)]
        public string Reason { get; set; }
    }
}
