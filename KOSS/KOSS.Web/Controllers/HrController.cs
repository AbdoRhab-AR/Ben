using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KOSS.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace KOSS.Web.Controllers
{
    [Authorize]
    public class HrController : Controller
    {
        private readonly AppDbContext _context;

        public HrController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var staff = await _context.StaffMembers.OrderBy(s => s.Role).ThenBy(s => s.FullName).ToListAsync();
            return View(staff);
        }

        public async Task<IActionResult> PayrollReport()
        {
            var staff = await _context.StaffMembers.Where(s => s.IsActive).OrderBy(s => s.Role).ToListAsync();
            return View(staff);
        }
    }
}
