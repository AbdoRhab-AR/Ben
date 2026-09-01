using System.Collections.Generic;
using System.Linq;
using KOSS.Web.Models;
using KOSS.Web.Models.ViewModels;

namespace KOSS.Web.Helpers
{
    // ============================================================
    //  محرك توزيع العربون - 70%
    //  يوزع الدفعة المدفوعة على الوحدات حسب الأولوية
    //  وفق سياسة الشركة: 70% من قيمة كل وحدة قبل إطلاق التصنيع
    // ============================================================
    public static class DepositAllocator
    {
        /// <summary>
        /// يوزع مبلغ العربون على وحدات العقد بالترتيب حسب الأولوية.
        /// كل وحدة تحتاج 70% من قيمتها لتُطلق للتصنيع.
        /// </summary>
        /// <param name="units">قائمة وحدات العقد مرتبة حسب الأولوية</param>
        /// <param name="depositAmount">مبلغ الدفعة الجديدة المُضافة</param>
        /// <param name="previouslyAllocated">ما تم تخصيصه مسبقاً لكل وحدة</param>
        /// <returns>قائمة نتائج التخصيص</returns>
        public static List<UnitAllocationResult> Allocate(
            List<KitchenUnit> units,
            decimal depositAmount)
        {
            var results = new List<UnitAllocationResult>();
            decimal remaining = depositAmount;

            // ترتيب الوحدات حسب الأولوية (الأقل رقماً = أعلى أولوية)
            var ordered = units.OrderBy(u => u.Priority).ToList();

            foreach (var unit in ordered)
            {
                decimal required   = unit.EstimatedValue * (unit.RequiredDepositPercentage / 100m);
                decimal alreadyHas = unit.AllocatedDeposit;
                decimal stillNeeds = required - alreadyHas;

                if (stillNeeds <= 0)
                {
                    // الوحدة ممولة بالفعل
                    unit.ManufacturingStatus = UnitManufacturingStatus.Active;
                    results.Add(new UnitAllocationResult
                    {
                        UnitName       = unit.UnitType.ToString(),
                        EstimatedValue = unit.EstimatedValue,
                        Required70Pct  = required,
                        Allocated      = alreadyHas,
                        Status         = "✅ نشطة - ممولة مسبقاً"
                    });
                    continue;
                }

                if (remaining >= stillNeeds)
                {
                    // نملك كفاية لإكمال هذه الوحدة
                    unit.AllocatedDeposit    += stillNeeds;
                    unit.ManufacturingStatus  = UnitManufacturingStatus.Active;
                    remaining                -= stillNeeds;

                    results.Add(new UnitAllocationResult
                    {
                        UnitName       = unit.UnitType.ToString(),
                        EstimatedValue = unit.EstimatedValue,
                        Required70Pct  = required,
                        Allocated      = unit.AllocatedDeposit,
                        Status         = "✅ نشطة - أُطلقت للتصنيع"
                    });
                }
                else if (remaining > 0)
                {
                    // دفعة جزئية فقط
                    unit.AllocatedDeposit    += remaining;
                    unit.ManufacturingStatus  = UnitManufacturingStatus.Suspended;
                    remaining                 = 0;

                    results.Add(new UnitAllocationResult
                    {
                        UnitName       = unit.UnitType.ToString(),
                        EstimatedValue = unit.EstimatedValue,
                        Required70Pct  = required,
                        Allocated      = unit.AllocatedDeposit,
                        Status         = $"⏸️ موقوفة - متبقي {stillNeeds - unit.AllocatedDeposit + alreadyHas:N3} د.ل"
                    });
                }
                else
                {
                    // لا يوجد رصيد
                    unit.ManufacturingStatus = UnitManufacturingStatus.Suspended;
                    results.Add(new UnitAllocationResult
                    {
                        UnitName       = unit.UnitType.ToString(),
                        EstimatedValue = unit.EstimatedValue,
                        Required70Pct  = required,
                        Allocated      = unit.AllocatedDeposit,
                        Status         = "🔴 موقوفة - لا يوجد رصيد"
                    });
                }
            }

            return results;
        }

        /// <summary>
        /// يحسب نسبة العربون المدفوعة لعقد ما
        /// </summary>
        public static decimal GetDepositPercentage(decimal paid, decimal total)
        {
            if (total == 0) return 0;
            return (paid / total) * 100m;
        }

        /// <summary>
        /// هل العقد مؤهل لإصدار أمر الشراء؟ (70% على الأقل للوحدة الرئيسية)
        /// </summary>
        public static bool IsEligibleForProduction(KitchenUnit unit)
        {
            return unit.AllocatedDeposit >= (unit.EstimatedValue * unit.RequiredDepositPercentage / 100m);
        }
    }
}
