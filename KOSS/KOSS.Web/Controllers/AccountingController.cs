using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KOSS.Web.Models;
using KOSS.Web.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace KOSS.Web.Controllers
{
    [Authorize]
    public class AccountingController : Controller
    {
        private readonly AppDbContext _context;

        public AccountingController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var requests = await _context.KitchenRequests
                .Include(r => r.Customer)
                .Include(r => r.Contracts)
                .Include(r => r.WorkOrders)
                .Include(r => r.Expenses)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var reports = requests.Select(ProfitabilityCalculator.Calculate).ToList();
            return View(reports);
        }

        public async Task<IActionResult> ProjectCosting(int id)
        {
            var req = await _context.KitchenRequests
                .Include(r => r.Customer)
                .Include(r => r.Contracts)
                    .ThenInclude(c => c.Payments)
                .Include(r => r.WorkOrders)
                    .ThenInclude(w => w.StockIssues)
                .Include(r => r.Expenses)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (req == null) return NotFound();

            var report = ProfitabilityCalculator.Calculate(req);
            ViewBag.Request = req;
            return View(report);
        }
    }
}
