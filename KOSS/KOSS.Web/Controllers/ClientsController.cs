using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KOSS.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace KOSS.Web.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {
        private readonly AppDbContext _context;

        public ClientsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search)
        {
            var query = _context.Customers.Include(c => c.KitchenRequests).AsQueryable();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.Name.Contains(search) || c.Phone.Contains(search) || c.District.Contains(search));

            var list = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
            ViewBag.Search = search;
            return View(list);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer model)
        {
            if (ModelState.IsValid)
            {
                var existing = await _context.Customers.FirstOrDefaultAsync(c => c.Phone == model.Phone.Trim());
                if (existing != null)
                {
                    ModelState.AddModelError("Phone", "رقم الهاتف مسجل مسبقاً لعميل آخر.");
                    return View(model);
                }

                model.CreatedBy = User.Identity?.Name ?? "Admin";
                _context.Customers.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"تم تسجيل العميل ({model.Name}) بنجاح.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Inquiries)
                .Include(c => c.KitchenRequests)
                    .ThenInclude(r => r.Contracts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null) return NotFound();
            return View(customer);
        }
    }
}
