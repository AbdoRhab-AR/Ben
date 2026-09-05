using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOSS.Web.Models
{
    public enum SiteVisitStatus
    {
        [Display(Name = "معاينة مجدولة")]
        Scheduled = 1,

        [Display(Name = "قيد التنفيذ")]
        InProgress = 2,

        [Display(Name = "تمت الزيارة الميدانية")]
        Completed = 3,

        [Display(Name = "القياسات بانتظار الاعتماد")]
        AwaitingReview = 4,

        [Display(Name = "قياسات معتمدة")]
        Approved = 5,

        [Display(Name = "معاينة ملغاة")]
        Cancelled = 6
    }

    public class SiteVisit
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "طلب المطبخ / المشروع")]
        public int KitchenRequestId { get; set; }

        [Display(Name = "مهندس المعاينة المكلف")]
        public int? AssignedSurveyorId { get; set; }

        [Display(Name = "الموعد المجدول للزيارة")]
        public DateTime ScheduledDate { get; set; } = DateTime.Now;

        [Display(Name = "تاريخ الزيارة الفعلي")]
        public DateTime? ActualVisitDate { get; set; }

        [Display(Name = "طول الجدار الرئيسي (سم)")]
        public decimal WallLength1Cm { get; set; } = 0;

        [Display(Name = "طول الجدار الثاني (سم)")]
        public decimal WallLength2Cm { get; set; } = 0;

        [Display(Name = "طول الجدار الثالث (سم)")]
        public decimal WallLength3Cm { get; set; } = 0;

        [Display(Name = "الارتفاع من الأرض للسقف (سم)")]
        public decimal CeilingHeightCm { get; set; } = 0;

        [Display(Name = "المساحة الإجمالية المقدرة (م²)")]
        public decimal EstimatedAreaM2 { get; set; } = 0;

        [Display(Name = "زاوية الجدران (درجة)")]
        public decimal CornerAngleDegrees { get; set; } = 90;

        [Display(Name = "هل الزوايا قائمة 90° بدون شطرة؟")]
        public bool HasSquareCorners { get; set; } = true;

        [Display(Name = "ارتفاع جلسة النافذة عن الأرض (سم)")]
        public decimal? WindowSillHeightCm { get; set; }

        [Display(Name = "موقع نقطة تصريف المياه والسباكة")]
        [StringLength(200)]
        public string PlumbingNotes { get; set; }

        [Display(Name = "مواقع مقابس وتغذية الكهرباء والشفاط")]
        [StringLength(200)]
        public string ElectricalNotes { get; set; }

        [Display(Name = "توزيع فيش الأجهزة المدمجة (فرن، غسالة، ثلاجة)")]
        [StringLength(300)]
        public string ApplianceOutletsNotes { get; set; }

        [Display(Name = "مواقع النوافذ والأبواب والأعمدة")]
        [StringLength(300)]
        public string StructuralObstacles { get; set; }

        [Display(Name = "ملاحظات العوائق الإنشائية")]
        [StringLength(300)]
        public string ObstaclesNotes { get; set; }

        [Display(Name = "التقرير الهندسي الميداني")]
        [StringLength(1000)]
        public string SurveyorReport { get; set; }

        [Display(Name = "ملاحظات المعاينة")]
        [StringLength(500)]
        public string Notes { get; set; }

        [Display(Name = "معتمد القياسات")]
        [StringLength(100)]
        public string ApprovedBy { get; set; }

        [Display(Name = "تاريخ الاعتماد")]
        public DateTime? ApprovedAt { get; set; }

        [Display(Name = "حالة المعاينة والقياسات")]
        public SiteVisitStatus Status { get; set; } = SiteVisitStatus.Scheduled;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }

        public virtual KitchenRequest KitchenRequest { get; set; }
        public virtual StaffMember AssignedSurveyor { get; set; }
    }
}
