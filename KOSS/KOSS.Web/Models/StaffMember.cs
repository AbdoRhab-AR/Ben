using System;
using System.ComponentModel.DataAnnotations;

namespace KOSS.Web.Models
{
    // ============================================================
    //  دور الموظف في النظام
    // ============================================================
    public enum StaffRole
    {
        [Display(Name = "موظف مبيعات")]
        SalesStaff = 1,

        [Display(Name = "مصمم داخلي")]
        Designer = 2,

        [Display(Name = "مساح ميداني")]
        FieldSurveyor = 3,

        [Display(Name = "مسؤول مالي")]
        Finance = 4,

        [Display(Name = "مدير مصنع")]
        FactoryManager = 5,

        [Display(Name = "مدير تنفيذي")]
        Executive = 6
    }

    // ============================================================
    //  نموذج الموظف - الحوافز والمسؤوليات
    // ============================================================
    public class StaffMember
    {
        public int Id { get; set; }

        [Display(Name = "معرّف المستخدم (Identity)")]
        public string UserId { get; set; }

        [Required]
        [Display(Name = "الاسم الكامل")]
        [StringLength(150)]
        public string FullName { get; set; }

        [Display(Name = "رقم الهاتف")]
        [StringLength(30)]
        public string Phone { get; set; }

        [Display(Name = "البريد الإلكتروني")]
        [StringLength(100)]
        public string Email { get; set; }

        [Display(Name = "الدور الوظيفي")]
        public StaffRole Role { get; set; }

        [Display(Name = "الراتب الأساسي (د.ل)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal BaseSalary { get; set; }

        [Display(Name = "رصيد المكافآت المتراكمة (د.ل)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal BonusBalance { get; set; } = 0;

        [Display(Name = "رصيد الخصومات/المسؤوليات المالية (د.ل)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal LiabilityBalance { get; set; } = 0;

        [Display(Name = "صافي المكافآت (د.ل)")]
        public decimal NetBonus => BonusBalance - LiabilityBalance;

        [Display(Name = "إجمالي الراتب الشهري (د.ل)")]
        public decimal TotalMonthlyPay => BaseSalary + (NetBonus > 0 ? NetBonus : 0);

        [Display(Name = "تاريخ الانضمام")]
        public DateTime JoinDate { get; set; } = DateTime.Now;

        [Display(Name = "هل نشط؟")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "ملاحظات")]
        [StringLength(500)]
        public string Notes { get; set; }
    }
}
