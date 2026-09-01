using System;
using System.Collections.Generic;

namespace KOSS.Web.Helpers
{
    // ============================================================
    //  حاسبة التداخل (Nesting) لألواح الخشب
    //  قياس اللوح القياسي: 2.80م × 1.22م = 3.416 م²
    // ============================================================
    public static class NestingCalculator
    {
        // أبعاد اللوح القياسي المستورد (بالمتر)
        public const decimal SheetLengthM  = 2.80m;
        public const decimal SheetWidthM   = 1.22m;
        public const decimal SheetAreaM2   = SheetLengthM * SheetWidthM; // 3.416 م²

        // هامش الفقد الافتراضي (عائد القطع %)
        public const decimal DefaultWastePercent = 15m;

        /// <summary>
        /// يحسب عدد الألواح المطلوبة بناء على المساحة الكلية للوحدة
        /// </summary>
        /// <param name="totalAreaM2">المساحة الكلية للأسطح المطلوبة بالمتر المربع</param>
        /// <param name="wastePercent">نسبة الهدر الإضافية (افتراضي 15%)</param>
        /// <returns>نتيجة حساب الألواح</returns>
        public static NestingResult Calculate(decimal totalAreaM2, decimal wastePercent = DefaultWastePercent)
        {
            if (totalAreaM2 <= 0)
                return new NestingResult { Error = "يجب إدخال مساحة أكبر من صفر" };

            // المساحة مع الهدر
            decimal wasteMultiplier   = 1 + (wastePercent / 100m);
            decimal areaWithWaste     = totalAreaM2 * wasteMultiplier;

            // عدد الألواح (نجمع للأعلى دائماً)
            decimal sheetsExact       = areaWithWaste / SheetAreaM2;
            int     sheetsRequired    = (int)Math.Ceiling(sheetsExact);

            // حساب المساحة المستخدمة والهدر
            decimal totalSheetArea    = sheetsRequired * SheetAreaM2;
            decimal usedArea          = totalAreaM2;
            decimal wasteArea         = totalSheetArea - usedArea;
            decimal actualWastePct    = totalSheetArea > 0 ? (wasteArea / totalSheetArea) * 100 : 0;

            return new NestingResult
            {
                TotalAreaM2       = totalAreaM2,
                WastePercent      = wastePercent,
                AreaWithWaste     = areaWithWaste,
                SheetsRequired    = sheetsRequired,
                TotalSheetArea    = totalSheetArea,
                WasteAreaM2       = wasteArea,
                ActualWastePercent = actualWastePct,
                SheetLengthM      = SheetLengthM,
                SheetWidthM       = SheetWidthM,
                SheetAreaM2       = SheetAreaM2
            };
        }

        /// <summary>
        /// يحسب تقدير تكلفة الألواح
        /// </summary>
        public static decimal EstimateCost(int sheetsRequired, decimal pricePerSheet)
        {
            return sheetsRequired * pricePerSheet;
        }
    }

    // ============================================================
    //  نتيجة حساب التداخل
    // ============================================================
    public class NestingResult
    {
        public decimal TotalAreaM2          { get; set; }  // المساحة المطلوبة
        public decimal WastePercent         { get; set; }  // نسبة الهدر المُضافة
        public decimal AreaWithWaste        { get; set; }  // المساحة مع الهدر
        public int     SheetsRequired       { get; set; }  // عدد الألواح المطلوبة
        public decimal TotalSheetArea       { get; set; }  // مساحة الألواح الكلية
        public decimal WasteAreaM2          { get; set; }  // مساحة الهدر الفعلي
        public decimal ActualWastePercent   { get; set; }  // نسبة الهدر الفعلية
        public decimal SheetLengthM         { get; set; }  // طول اللوح
        public decimal SheetWidthM          { get; set; }  // عرض اللوح
        public decimal SheetAreaM2          { get; set; }  // مساحة اللوح الواحد
        public string  Error                { get; set; }  // رسالة خطأ (إن وجدت)
        public bool    IsValid              => string.IsNullOrEmpty(Error);
    }
}
