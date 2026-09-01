using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOSS.Web.Models
{
    // ============================================================
    //  المعاينة والقياسات الميدانية (SiteVisit)
    // ============================================================
    public class SiteVisit
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "طلب المطبخ")]
        public int KitchenRequestId { get; set; }

        [Display(Name = "مهندس المعاينة المكلف")]
        public int? AssignedSurveyorId { get; set; }

        [Display(Name = "الموعد المجدول للزيارة")]
        public DateTime? ScheduledDate { get; set; }

        [Display(Name = "تاريخ الزيارة الفعلي")]
        public DateTime? ActualVisitDate { get; set; }

        // تفاصيل القياسات الفنية
        [Display(Name = "طول الجدار الرئيسي (سم)")]
        public decimal? WallLength1Cm { get; set; }

        [Display(Name = "طول الجدار الثاني (سم)")]
        public decimal? WallLength2Cm { get; set; }

        [Display(Name = "طول الجدار الثالث (سم)")]
        public decimal? WallLength3Cm { get; set; }

        [Display(Name = "الارتفاع من الأرض للسقف (سم)")]
        public decimal? CeilingHeightCm { get; set; }

        [Display(Name = "المساحة الإجمالية المقدرة (م²)")]
        public decimal? EstimatedAreaM2 { get; set; }

        [Display(Name = "موقع نقطة تصريف المياه والسباكة")]
        [StringLength(200)]
        public string PlumbingNotes { get; set; }

        [Display(Name = "مواقع مقابس وتغذية الكهرباء")]
        [StringLength(200)]
        public string ElectricalNotes { get; set; }

        [Display(Name = "مواقع النوافذ والأبواب والأعمدة")]
        [StringLength(300)]
        public string ObstaclesNotes { get; set; }

        [Display(Name = "رابط ملف المخطط / الصور المرفقة")]
        [StringLength(500)]
        public string AttachmentsPath { get; set; }

        [Display(Name = "تقرير المهندس والملاحظات")]
        [StringLength(1000)]
        public string SurveyorReport { get; set; }

        [Display(Name = "حالة المعاينة")]
        public SiteVisitStatus Status { get; set; } = SiteVisitStatus.Scheduled;

        [Display(Name = "المعتمد")]
        public string ApprovedBy { get; set; }

        [Display(Name = "تاريخ الاعتماد")]
        public DateTime? ApprovedAt { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // العلاقات
        [ForeignKey("KitchenRequestId")]
        public virtual KitchenRequest KitchenRequest { get; set; }

        [ForeignKey("AssignedSurveyorId")]
        public virtual StaffMember AssignedSurveyor { get; set; }
    }
}
