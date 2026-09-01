using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KOSS.Web.Models;

namespace KOSS.Web.Models.ViewModels
{
    // ============================================================
    //  ViewModel إنشاء عقد جديد
    // ============================================================
    public class CreateContractViewModel
    {
        public int ClientId { get; set; }

        [Display(Name = "اسم العميل")]
        public string ClientName { get; set; }

        [Display(Name = "سعر المتر (د.ل)")]
        [Required(ErrorMessage = "سعر المتر مطلوب")]
        [Range(0.001, 999999, ErrorMessage = "يجب أن يكون السعر أكبر من صفر")]
        public decimal PricePerMeter { get; set; }

        [Display(Name = "الوحدات المطلوبة")]
        public List<UnitSelectionItem> Units { get; set; } = new List<UnitSelectionItem>
        {
            new UnitSelectionItem { UnitType = UnitType.Kitchen, Selected = true, Priority = 1 }
        };

        [Display(Name = "ملاحظات")]
        [StringLength(500)]
        public string Notes { get; set; }
    }

    // ============================================================
    //  عنصر اختيار الوحدة في نموذج العقد
    // ============================================================
    public class UnitSelectionItem
    {
        public UnitType UnitType      { get; set; }
        public bool     Selected      { get; set; }
        public decimal  EstimatedValue { get; set; }
        public int      Priority      { get; set; }

        [Display(Name = "الوصف")]
        public string Description { get; set; }
    }
}
