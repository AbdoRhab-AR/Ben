using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KOSS.Web.Models
{
    // ============================================================
    //  حالة أمر الشراء
    // ============================================================
    public enum PurchaseOrderStatus
    {
        [Display(Name = "مسودة")]
        Draft = 1,

        [Display(Name = "أُرسل للمستودع")]
        SentToWarehouse = 2,

        [Display(Name = "أُرسل للمحاسبة")]
        SentToAccounting = 3,

        [Display(Name = "تمت الموافقة")]
        Approved = 4,

        [Display(Name = "صُرِّف للمصنع")]
        IssuedToFactory = 5,

        [Display(Name = "مكتمل")]
        Completed = 6
    }

    // ============================================================
    //  نموذج أمر الشراء / طلب الصرف
    // ============================================================
    public class PurchaseOrder
    {
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }

        [Display(Name = "رقم أمر الشراء")]
        public string PoNumber { get; set; }

        [Display(Name = "الحالة")]
        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;

        [Display(Name = "إجمالي التكلفة التقديرية (د.ل)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal TotalEstimatedCost { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "تاريخ الإرسال للمستودع")]
        public DateTime? SentToWarehouseAt { get; set; }

        [Display(Name = "تاريخ الإرسال للمحاسبة")]
        public DateTime? SentToAccountingAt { get; set; }

        [Display(Name = "تاريخ الصرف للمصنع")]
        public DateTime? IssuedToFactoryAt { get; set; }

        [Display(Name = "أُنشئ بواسطة")]
        public string CreatedBy { get; set; }

        [Display(Name = "ملاحظات")]
        [StringLength(500)]
        public string Notes { get; set; }

        // العلاقات
        public virtual Contract Contract { get; set; }
        public virtual ICollection<BomItem> BomItems { get; set; } = new List<BomItem>();
    }
}
