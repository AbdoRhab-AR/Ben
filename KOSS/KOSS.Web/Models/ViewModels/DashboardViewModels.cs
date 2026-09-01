using System.Collections.Generic;
using KOSS.Web.Models;

namespace KOSS.Web.Models.ViewModels
{
    // ============================================================
    //  لوحة التحكم التنفيذية - إحصائيات عامة
    // ============================================================
    public class DashboardViewModel
    {
        // إحصائيات العملاء
        public int TotalClients         { get; set; }
        public int InterestedClients    { get; set; }
        public int NotInterestedClients { get; set; }

        // إحصائيات العقود
        public int TotalContracts       { get; set; }
        public int ActiveContracts      { get; set; }
        public int SuspendedContracts   { get; set; }
        public int CompletedContracts   { get; set; }

        // إحصائيات مالية
        public decimal TotalContractValue  { get; set; }
        public decimal TotalCollected      { get; set; }
        public decimal TotalOutstanding    { get; set; }

        // مراحل خط الإنتاج (Pipeline)
        public int NewCount             { get; set; }
        public int MeasuredCount        { get; set; }
        public int DesignedCount        { get; set; }
        public int UnderProductionCount { get; set; }
        public int ManufacturedCount    { get; set; }
        public int InstalledCount       { get; set; }

        // العقود المتأخرة / الموقوفة
        public List<Contract> StalledContracts { get; set; } = new List<Contract>();

        // أحدث العملاء
        public List<Client>   RecentClients   { get; set; } = new List<Client>();

        // أحدث المدفوعات
        public List<Payment>  RecentPayments  { get; set; } = new List<Payment>();
    }
}
