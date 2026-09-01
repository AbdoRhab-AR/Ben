using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using KOSS.Web.Models;

namespace KOSS.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly KossDbContext db = new KossDbContext();

        public ActionResult Index()
        {
            DateTime today = DateTime.Today;
            DateTime endOfWeek = today.AddDays(7);

            // 1. المؤشرات الإجرائية التفاعلية (Actionable Items)
            ViewBag.NewInquiriesCount = db.CustomerInquiries.Count(i => i.Status == InquiryStatus.New || i.Status == InquiryStatus.Contacted);
            ViewBag.TodayVisitsCount = db.SiteVisits.Count(s => s.Status == SiteVisitStatus.Scheduled && DbFunctions.TruncateTime(s.ScheduledDate) == today);
            ViewBag.PendingDesignsCount = db.DesignVersions.Count(d => d.Status == DesignVersionStatus.Draft || d.Status == DesignVersionStatus.InProgress || d.Status == DesignVersionStatus.SentToCustomer);
            ViewBag.QuotationsWaitingResponseCount = db.Quotations.Count(q => q.Status == QuotationStatus.SentToCustomer);
            ViewBag.ContractsWaitingDepositCount = db.Contracts.Count(c => c.Status == ContractStatus.AwaitingDeposit);
            ViewBag.LowStockItemsCount = db.StockItems.Count(s => (s.PhysicalQuantity - s.ReservedQuantity) <= s.ItemMaster.ReorderLevel);
            ViewBag.ProjectsInManufacturingCount = db.KitchenRequests.Count(r => r.Status == KitchenRequestStatus.InManufacturing);
            ViewBag.WeeklyInstallationsCount = db.InstallationOrders.Count(i => i.ScheduledDate >= today && i.ScheduledDate <= endOfWeek);
            ViewBag.UncollectedBalancesCount = db.Contracts.Count(c => c.TotalValue > c.TotalPaid && c.Status >= ContractStatus.Active);
            ViewBag.OpenSnagsCount = db.SnagItems.Count(s => !s.IsResolved);

            // 2. إجمالي المبالغ المالية
            ViewBag.TotalContractRevenue = db.Contracts.Any() ? db.Contracts.Sum(c => c.TotalValue) : 0;
            ViewBag.TotalCollected = db.Contracts.Any() ? db.Contracts.Sum(c => c.TotalPaid) : 0;
            ViewBag.TotalUncollected = db.Contracts.Any() ? db.Contracts.Sum(c => c.TotalValue - c.TotalPaid) : 0;

            // 3. طلبات المطبخ النشطة في خط السير (Pipeline)
            var activeRequests = db.KitchenRequests
                .Include(r => r.Customer)
                .Include(r => r.AssignedSalesStaff)
                .Include(r => r.Contracts)
                .Where(r => r.Status != KitchenRequestStatus.Closed && r.Status != KitchenRequestStatus.CancelledOrRejected)
                .OrderByDescending(r => r.UpdatedAt)
                .Take(10)
                .ToList();

            // 4. المعاينات القريبة
            ViewBag.UpcomingVisits = db.SiteVisits
                .Include(s => s.KitchenRequest.Customer)
                .Include(s => s.AssignedSurveyor)
                .Where(s => s.Status == SiteVisitStatus.Scheduled)
                .OrderBy(s => s.ScheduledDate)
                .Take(5)
                .ToList();

            // 5. الأصناف تحت حد الطلب
            ViewBag.LowStockList = db.StockItems
                .Include(s => s.Warehouse)
                .Include(s => s.ItemMaster)
                .Where(s => (s.PhysicalQuantity - s.ReservedQuantity) <= s.ItemMaster.ReorderLevel)
                .Take(5)
                .ToList();

            return View(activeRequests);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
