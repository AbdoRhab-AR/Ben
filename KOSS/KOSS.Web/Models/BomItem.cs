using System;
using System.ComponentModel.DataAnnotations;

namespace KOSS.Web.Models
{
    // ============================================================
    //  بند في قائمة المواد (BOM)
    //  يرتبط بـ 104 كود معياري للمواد والإكسسوارات
    // ============================================================
    public class BomItem
    {
        public int Id { get; set; }

        [Required]
        public int KitchenUnitId { get; set; }

        public int? PurchaseOrderId { get; set; }

        [Display(Name = "كود الصنف (1-104)")]
        [Range(1, 999)]
        public int ItemCode { get; set; }

        [Display(Name = "اسم الصنف")]
        [Required, StringLength(200)]
        public string ItemName { get; set; }

        [Display(Name = "الفئة")]
        [StringLength(100)]
        public string Category { get; set; }  // مثال: ألواح خشب، مفصلات، رولات...

        [Display(Name = "الوحدة")]
        [StringLength(30)]
        public string Unit { get; set; } = "قطعة";

        [Display(Name = "الكمية المطلوبة")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal QuantityRequired { get; set; }

        [Display(Name = "الكمية المُصرَّفة")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal QuantityIssued { get; set; } = 0;

        [Display(Name = "سعر الوحدة (د.ل)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal UnitCost { get; set; }

        [Display(Name = "الإجمالي (د.ل)")]
        public decimal TotalCost => QuantityRequired * UnitCost;

        [Display(Name = "تم الإصدار للمصنع؟")]
        public bool IssuedToFactory { get; set; } = false;

        [Display(Name = "ملاحظات")]
        [StringLength(300)]
        public string Notes { get; set; }

        [Display(Name = "تاريخ الإضافة")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // العلاقات
        public virtual KitchenUnit KitchenUnit { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
    }
}
