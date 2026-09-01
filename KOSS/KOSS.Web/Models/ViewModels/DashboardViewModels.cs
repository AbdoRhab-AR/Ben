using System.Collections.Generic;
using KOSS.Web.Models;

namespace KOSS.Web.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalRequests { get; set; }
        public int ActiveInquiries { get; set; }
        public int ScheduledSiteVisits { get; set; }
        public int ActiveDesigns { get; set; }
        public int PendingQuotations { get; set; }
        public int ActiveContracts { get; set; }
        public int ActiveWorkOrders { get; set; }
        public int ReadyForInstallation { get; set; }
        public int InInstallation { get; set; }

        public decimal TotalContractValue { get; set; }
        public decimal TotalCollected { get; set; }
        public decimal TotalOutstanding => TotalContractValue - TotalCollected;

        public List<KitchenRequest> RecentRequests { get; set; } = new List<KitchenRequest>();
    }
}
