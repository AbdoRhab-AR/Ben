using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KOSS.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KOSS.Web.Controllers
{
    [Authorize(Roles = "Executive,Admin")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            await SeedPermissionsAsync();
            var users = await _context.Users.Include(u => u.UserPermissions).ThenInclude(p => p.Permission).ToListAsync();
            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم إنشاء المستخدم بنجاح.";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        private async Task SeedPermissionsAsync()
        {
            if (!await _context.Permissions.AnyAsync())
            {
                var permissions = new List<Permission>
                {
                    new Permission { Name = "ManageRequests", DisplayName = "إدارة طلبات المطابخ", Description = "صلاحية فتح وتعديل ومتابعة طلبات المطابخ" },
                    new Permission { Name = "ManageDesigns", DisplayName = "إدارة التصاميم", Description = "صلاحية رفع واعتماد مخططات الـ 3D" },
                    new Permission { Name = "ManageQuotations", DisplayName = "إدارة عروض الأسعار", Description = "صلاحية تحرير واعتماد عروض الأسعار" },
                    new Permission { Name = "ManageContracts", DisplayName = "إدارة العقود والدفعات", Description = "صلاحية توقيع العقود وقبض الدفعات" },
                    new Permission { Name = "ManageFactory", DisplayName = "إدارة المصنع والتشغيل", Description = "صلاحية متابعة خطوط الإنتاج وفحص الجودة" },
                    new Permission { Name = "ManageInventory", DisplayName = "إدارة المستودعات", Description = "صلاحية الصرف والاستلام المخزني" },
                    new Permission { Name = "ManageAccounting", DisplayName = "الإدارة المالية والتكاليف", Description = "صلاحية استعراض تقارير الأرباح وإغلاق المشاريع" }
                };

                _context.Permissions.AddRange(permissions);
                await _context.SaveChangesAsync();
            }
        }
    }
}
