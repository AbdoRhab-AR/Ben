using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOSS.Web.Models
{
    // ============================================================
    //  مهام ومراحل التصنيع في المصنع (ManufacturingTask)
    // ============================================================
    public class ManufacturingTask
    {
        public int Id { get; set; }

        [Required]
        public int WorkOrderId { get; set; }

        [Display(Name = "اسم المرحلة / المهمة")]
        [Required, StringLength(150)]
        public string TaskName { get; set; } // تقطيع الألواح، شريط حرف PVC، تجميع الخزائن، تثبيت المفصلات والسكك، التغليف

        [Display(Name = "الفني المسؤول")]
        [StringLength(100)]
        public string TechnicianName { get; set; }

        [Display(Name = "تاريخ البدء الفعلي")]
        public DateTime? StartedAt { get; set; }

        [Display(Name = "تاريخ الإنجاز الفعلي")]
        public DateTime? CompletedAt { get; set; }

        [Display(Name = "الحالة")]
        public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, ReworkRequired

        [Display(Name = "ملاحظات الفني / المشرف")]
        [StringLength(500)]
        public string Notes { get; set; }

        // العلاقة
        public virtual WorkOrder WorkOrder { get; set; }
    }

    // ============================================================
    //  فحص الجودة والمطابقة بالمصنع (QualityCheck)
    // ============================================================
    public class QualityCheck
    {
        public int Id { get; set; }

        [Required]
        public int WorkOrderId { get; set; }

        [Display(Name = "رقم تقرير فحص الجودة")]
        [Required, StringLength(50)]
        public string ReportNumber { get; set; }

        [Display(Name = "تاريخ الفحص")]
        public DateTime InspectionDate { get; set; } = DateTime.Now;

        [Display(Name = "المفتش / مهندس الجودة")]
        [Required, StringLength(100)]
        public string InspectorName { get; set; }

        [Display(Name = "فحص مطابقة المقاسات للمخطط")]
        public bool DimensionsMatched { get; set; } = true;

        [Display(Name = "فحص سلامة الأسطح والخلو من الخدوش")]
        public bool SurfacesFlawless { get; set; } = true;

        [Display(Name = "فحص حركة الأدراج والمفصلات")]
        public bool HardwareWorkingSmoothly { get; set; } = true;

        [Display(Name = "فحص التغليف وحماية الحواف")]
        public bool PackagingSecured { get; set; } = true;

        [Display(Name = "النتيجة النهائية للفحص")]
        public bool Passed { get; set; } = true;

        [Display(Name = "ملاحظات الجودة وتوجيهات التعديل")]
        [StringLength(1000)]
        public string Notes { get; set; }

        // العلاقة
        public virtual WorkOrder WorkOrder { get; set; }
        public virtual ICollection<SnagItem> SnagItems { get; set; } = new List<SnagItem>();
    }

    // ============================================================
    //  ملاحظة / عيب / نقص يحتاج معالجة (SnagItem)
    // ============================================================
    public class SnagItem
    {
        public int Id { get; set; }

        [Display(Name = "فحص الجودة")]
        public int? QualityCheckId { get; set; }

        [Display(Name = "طلب المطبخ")]
        public int? KitchenRequestId { get; set; }

        [Display(Name = "وصف الملاحظة أو النقص")]
        [Required, StringLength(300)]
        public string Description { get; set; }

        [Display(Name = "المسؤول عن المعالجة")]
        [StringLength(100)]
        public string AssignedTo { get; set; }

        [Display(Name = "تاريخ الرصد")]
        public DateTime LoggedAt { get; set; } = DateTime.Now;

        [Display(Name = "هل تم حل ومعالجة الملاحظة؟")]
        public bool IsResolved { get; set; } = false;

        [Display(Name = "تاريخ المعالجة والحل")]
        public DateTime? ResolvedAt { get; set; }

        // العلاقات
        public virtual QualityCheck QualityCheck { get; set; }
        public virtual KitchenRequest KitchenRequest { get; set; }
    }

    // ============================================================
    //  أمر التركيب الميداني (InstallationOrder)
    // ============================================================
    public class InstallationOrder
    {
        public int Id { get; set; }

        [Required]
        public int WorkOrderId { get; set; }

        [Display(Name = "رقم أمر التركيب")]
        [Required, StringLength(50)]
        public string OrderNumber { get; set; }

        [Display(Name = "تاريخ التركيب المجدول")]
        public DateTime ScheduledDate { get; set; }

        [Display(Name = "رئيس فريق التركيب")]
        [Required, StringLength(100)]
        public string TeamLeadName { get; set; }

        [Display(Name = "رقم وسيلة النقل / الشاحنة")]
        [StringLength(50)]
        public string VehicleNumber { get; set; }

        [Display(Name = "عدد الأمتار الطولية المنجزة بالتركيب")]
        public decimal InstalledLinearMeters { get; set; } = 0;

        [Display(Name = "حالة التركيب")]
        public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, CompletedWithSnags, FullyCompleted

        [Display(Name = "تقرير التركيب الميداني")]
        [StringLength(1000)]
        public string InstallationReport { get; set; }

        // العلاقة
        public virtual WorkOrder WorkOrder { get; set; }
    }

    // ============================================================
    //  محضر التسليم النهائي (HandoverDocument)
    // ============================================================
    public class HandoverDocument
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "طلب المطبخ")]
        public int KitchenRequestId { get; set; }

        [Display(Name = "أمر التنفيذ")]
        public int? WorkOrderId { get; set; }

        [Display(Name = "رقم محضر التسليم")]
        [Required, StringLength(50)]
        public string DocumentNumber { get; set; }

        [Display(Name = "تاريخ وساعة التسليم")]
        public DateTime HandoverDate { get; set; } = DateTime.Now;

        [Display(Name = "اسم ممثل الشركة بالتسليم")]
        [Required, StringLength(100)]
        public string CompanyRepresentative { get; set; }

        [Display(Name = "اسم العميل المستلم")]
        [Required, StringLength(150)]
        public string CustomerSignerName { get; set; }

        [Display(Name = "هل تم قبول الأعمال بنجاح؟")]
        public bool CustomerAccepted { get; set; } = true;

        [Display(Name = "ملاحظات العميل الختامية")]
        [StringLength(500)]
        public string CustomerRemarks { get; set; }

        [Display(Name = "رابط ملف محضر التسليم الموقع ممسوحاً ضوئياً")]
        [StringLength(500)]
        public string SignedDocumentUrl { get; set; }

        // العلاقات
        public virtual KitchenRequest KitchenRequest { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }
    }

    // ============================================================
    //  مصروفات المشروع المباشرة (ProjectExpense)
    // ============================================================
    public class ProjectExpense
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "طلب المطبخ / مركز التكلفة")]
        public int KitchenRequestId { get; set; }

        [Display(Name = "نوع المصروف")]
        [Required, StringLength(100)]
        public string ExpenseType { get; set; } // نقل وشحن، مقاول تركيب خارجي، عمالة طارئة، إكسسوارات طارئة، رافعة...

        [Display(Name = "المبلغ المدفوع (د.ل)")]
        public decimal Amount { get; set; }

        [Display(Name = "تاريخ الدفع")]
        public DateTime ExpenseDate { get; set; } = DateTime.Now;

        [Display(Name = "المستفيد / المدفوع له")]
        [StringLength(150)]
        public string PaidTo { get; set; }

        [Display(Name = "رقم الفاتورة / الإيصال المرجعي")]
        [StringLength(100)]
        public string ReceiptReference { get; set; }

        [Display(Name = "الشرح والتفاصيل")]
        [StringLength(500)]
        public string Description { get; set; }

        [Display(Name = "المعتمد")]
        public string ApprovedBy { get; set; }

        // العلاقة
        public virtual KitchenRequest KitchenRequest { get; set; }
    }
}
