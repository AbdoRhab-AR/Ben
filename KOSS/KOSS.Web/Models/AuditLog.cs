using System;
using System.ComponentModel.DataAnnotations;

namespace KOSS.Web.Models
{
    // ============================================================
    //  سجل التدقيق - يتتبع كل تغيير في البيانات
    // ============================================================
    public class AuditLog
    {
        public int Id { get; set; }

        [Display(Name = "اسم الجدول")]
        [StringLength(100)]
        public string TableName { get; set; }

        [Display(Name = "معرّف السجل")]
        public int RecordId { get; set; }

        [Display(Name = "نوع العملية")]
        [StringLength(20)]
        public string Action { get; set; }  // Create / Update / Delete / StatusChange

        [Display(Name = "القيمة القديمة")]
        public string OldValue { get; set; }

        [Display(Name = "القيمة الجديدة")]
        public string NewValue { get; set; }

        [Display(Name = "وصف التغيير")]
        [StringLength(300)]
        public string Description { get; set; }

        [Display(Name = "تغيير بواسطة")]
        [StringLength(200)]
        public string ChangedBy { get; set; }

        [Display(Name = "تاريخ ووقت التغيير")]
        public DateTime ChangedAt { get; set; } = DateTime.Now;

        [Display(Name = "عنوان IP")]
        [StringLength(50)]
        public string IpAddress { get; set; }
    }
}
