using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KOSS.Web.Models
{
    // ============================================================
    //  حالة العميل
    // ============================================================
    public enum ClientStatus
    {
        [Display(Name = "مهتم")]
        Interested = 1,

        [Display(Name = "غير مهتم")]
        NotInterested = 2
    }

    // ============================================================
    //  نموذج العميل
    // ============================================================
    public class Client
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "الاسم مطلوب")]
        [Display(Name = "اسم العميل")]
        [StringLength(150)]
        public string Name { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Display(Name = "رقم الهاتف")]
        [StringLength(20)]
        public string Phone { get; set; }

        [Display(Name = "رقم هاتف إضافي")]
        [StringLength(20)]
        public string Phone2 { get; set; }

        [Display(Name = "العنوان")]
        [StringLength(300)]
        public string Address { get; set; }

        [Display(Name = "المنطقة / الحي")]
        [StringLength(100)]
        public string District { get; set; }

        [Display(Name = "الحالة")]
        public ClientStatus Status { get; set; } = ClientStatus.Interested;

        [Display(Name = "ملاحظات")]
        [StringLength(500)]
        public string Notes { get; set; }

        [Display(Name = "تاريخ التسجيل")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "مسجَّل بواسطة")]
        public string CreatedBy { get; set; }

        // العلاقات
        public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    }
}
