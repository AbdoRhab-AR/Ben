using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KOSS.Web.Models;
using KOSS.Web.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KOSS.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardViewModel
            {
                TotalRequests = await _context.KitchenRequests.CountAsync(),
                ActiveInquiries = await _context.CustomerInquiries.CountAsync(i => i.Status == InquiryStatus.New || i.Status == InquiryStatus.Contacted),
                ScheduledSiteVisits = await _context.SiteVisits.CountAsync(v => v.Status == SiteVisitStatus.Scheduled),
                ActiveDesigns = await _context.DesignVersions.CountAsync(d => d.Status != DesignVersionStatus.ApprovedByCustomer),
                PendingQuotations = await _context.Quotations.CountAsync(q => q.Status == QuotationStatus.SentToCustomer),
                ActiveContracts = await _context.Contracts.CountAsync(c => c.Status == ContractStatus.Active),
                ActiveWorkOrders = await _context.WorkOrders.CountAsync(w => w.Status == WorkOrderStatus.Manufacturing || w.Status == WorkOrderStatus.Planning),
                ReadyForInstallation = await _context.KitchenRequests.CountAsync(r => r.Status == KitchenRequestStatus.ReadyForInstallation),
                InInstallation = await _context.KitchenRequests.CountAsync(r => r.Status == KitchenRequestStatus.InInstallation),
                TotalContractValue = await _context.Contracts.Where(c => c.Status == ContractStatus.Active || c.Status == ContractStatus.Completed).SumAsync(c => (decimal?)c.TotalValue) ?? 0,
                TotalCollected = await _context.Contracts.Where(c => c.Status == ContractStatus.Active || c.Status == ContractStatus.Completed).SumAsync(c => (decimal?)c.TotalPaid) ?? 0,
                RecentRequests = await _context.KitchenRequests
                    .Include(r => r.Customer)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(10)
                    .ToListAsync()
            };

            return View(vm);
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }
    }
}
