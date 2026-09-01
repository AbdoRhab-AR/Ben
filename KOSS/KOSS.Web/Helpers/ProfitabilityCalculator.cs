using System;
using System.Linq;
using KOSS.Web.Models;

namespace KOSS.Web.Helpers
{
    // ============================================================
    //  تقرير وتحليل الربحية الفعلي للمشروع (Project Profitability)
    // ============================================================
    public class ProjectProfitabilityReport
    {
        public int KitchenRequestId { get; set; }
        public string RequestNumber { get; set; }
        public string CustomerName { get; set; }

        // 1. الإيرادات
        public decimal ContractRevenue { get; set; }
        public decimal TotalCollected { get; set; }
        public decimal RemainingBalance { get; set; }

        // 2. التكاليف المباشرة
        public decimal MaterialCost { get; set; }          // تكلفة المواد المصروفة من المخزن
        public decimal DirectPurchasesCost { get; set; }   // تكلفة المشتريات المباشرة للمشروع
        public decimal InstallationLaborCost { get; set; } // أجور وفنيو التركيب
        public decimal DirectExpensesCost { get; set; }    // مصروفات إضافية (نقل، رافعة، ضيافة...)

        // 3. الإجماليات والربحية
        public decimal TotalProjectCost => MaterialCost + DirectPurchasesCost + InstallationLaborCost + DirectExpensesCost;
        public decimal NetProfit => ContractRevenue - TotalProjectCost;
        public decimal ProfitMarginPercentage => ContractRevenue > 0 ? (NetProfit / ContractRevenue) * 100 : 0;
        public bool IsProfitable => NetProfit > 0;
    }

    public static class ProfitabilityCalculator
    {
        public static ProjectProfitabilityReport Calculate(KitchenRequest request)
        {
            var report = new ProjectProfitabilityReport
            {
                KitchenRequestId = request.Id,
                RequestNumber = request.RequestNumber,
                CustomerName = request.Customer != null ? request.Customer.Name : "-"
            };

            var contract = request.ActiveContract ?? request.Contracts?.FirstOrDefault();
            if (contract != null)
            {
                report.ContractRevenue = contract.TotalValue;
                report.TotalCollected = contract.TotalPaid;
                report.RemainingBalance = contract.RemainingBalance;
            }

            var wo = request.CurrentWorkOrder ?? request.WorkOrders?.FirstOrDefault();
            if (wo != null)
            {
                // تكلفة المواد المصروفة من المخزن
                if (wo.StockIssues != null)
                {
                    report.MaterialCost = wo.StockIssues.Sum(s => s.TotalCost);
                }

                // أجور التركيب (50 د.ل لكل متر طولي مركب كمعيار)
                if (wo.InstallationOrders != null)
                {
                    decimal totalMeters = wo.InstallationOrders.Sum(i => i.InstalledLinearMeters);
                    report.InstallationLaborCost = totalMeters * 50m;
                }
            }

            // المصروفات المباشرة المرتبطة بالطلب
            if (request.Expenses != null)
            {
                report.DirectExpensesCost = request.Expenses.Sum(e => e.Amount);
            }

            return report;
        }
    }
}
