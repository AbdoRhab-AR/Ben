using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KOSS.Web.Models
{
    // ============================================================
    //  نوع الوحدة المصممة
    // ============================================================
    public enum UnitType
    {
        [Display(Name = "مطبخ رئيسي")]
        Kitchen = 1,

        [Display(Name = "غرفة نوم")]
        Bedroom = 2,

        [Display(Name = "غرفة ملابس (دريسينج)")]
        DressingRoom = 3,

        [Display(Name = "غرفة غسيل")]
        Laundry = 4,

        [Display(Name = "حمام")]
        Bathroom = 5,

        [Display(Name = "مكتب / مكتبة")]
        Office = 6,

        [Display(Name = "أخرى")]
        Other = 99
    }

    // ============================================================
    //  حالة التصنيع للوحدة
    // ============================================================
    public enum UnitManufacturingStatus
    {
        [Display(Name = "في الانتظار")]
        Pending = 1,

        [Display(Name = "نشطة - جاري التصنيع")]
        Active = 2,

        [Display(Name = "موقوفة - الدفعة غير مكتملة")]
        Suspended = 3,

        [Display(Name = "تم التصنيع")]
        Manufactured = 4,

        [Display(Name = "تم التركيب")]
        Installed = 5,

        [Display(Name = "مكتملة")]
        Completed = 6
    }

    // ============================================================
    //  نموذج الوحدة (مطبخ / غرفة / دريسينج ...)
    // ============================================================
    public class KitchenUnit
    {
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }

        [Display(Name = "نوع الوحدة")]
        public UnitType UnitType { get; set; } = UnitType.Kitchen;

        [Display(Name = "وصف إضافي")]
        [StringLength(200)]
        public string Description { get; set; }

        [Display(Name = "القيمة التقديرية (د.ل)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal EstimatedValue { get; set; }

        [Display(Name = "نسبة العربون المطلوبة (%)")]
        public decimal RequiredDepositPercentage { get; set; } = 70m;

        [Display(Name = "العربون المطلوب (د.ل)")]
        public decimal RequiredDeposit => EstimatedValue * RequiredDepositPercentage / 100;

        [Display(Name = "العربون المُخصَّص (د.ل)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal AllocatedDeposit { get; set; }

        [Display(Name = "حالة التصنيع")]
        public UnitManufacturingStatus ManufacturingStatus { get; set; } = UnitManufacturingStatus.Pending;

        [Display(Name = "الأولوية (1=الأعلى)")]
        public int Priority { get; set; } = 1;

        // بيانات القياس
        [Display(Name = "المساحة الكلية (متر مربع)")]
        public decimal TotalArea { get; set; }

        [Display(Name = "الطول (سم)")]
        public decimal LengthCm { get; set; }

        [Display(Name = "العرض (سم)")]
        public decimal WidthCm { get; set; }

        [Display(Name = "الارتفاع (سم)")]
        public decimal HeightCm { get; set; }

        // بيانات التصميم
        [Display(Name = "ملف التصميم (مسار)")]
        public string DesignFilePath { get; set; }

        [Display(Name = "تاريخ التصميم")]
        public DateTime? DesignedAt { get; set; }

        [Display(Name = "المصمم")]
        public string DesignedBy { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // العلاقات
        public virtual Contract Contract { get; set; }
        public virtual ICollection<BomItem> BomItems { get; set; } = new List<BomItem>();
    }
}
