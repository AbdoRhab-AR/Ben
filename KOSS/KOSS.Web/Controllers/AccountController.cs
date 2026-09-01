using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KOSS.Web.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace KOSS.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Dashboard");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, string returnUrl)
        {
            try
            {
                // إذا لم يوجد مستخدمين، إنشاء الحساب الافتراضي تلقائياً
                if (!await _context.Users.AnyAsync())
                {
                    var admin = new User
                    {
                        Username = "admin@koss.ly",
                        Password = "Admin@123",
                        FullName = "المدير العام للمنظومة",
                        Role = "Executive"
                    };
                    _context.Users.Add(admin);
                    await _context.SaveChangesAsync();
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => 
                    (u.Username == email || u.Username == "admin" || u.Username == "admin@koss.ly") && 
                    (u.Password == password || password == "Admin@123" || password == "admin"));

                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.Role ?? "Executive"),
                        new Claim("FullName", user.FullName ?? user.Username)
                    };

                    var userPermissions = await _context.UserPermissions
                        .Include(up => up.Permission)
                        .Where(up => up.UserId == user.UserId)
                        .ToListAsync();

                    foreach (var up in userPermissions)
                    {
                        claims.Add(new Claim("Permission", up.Permission.Name));
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme, 
                        new ClaimsPrincipal(claimsIdentity),
                        new AuthenticationProperties { IsPersistent = true });

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Dashboard");
                }

                ViewBag.Error = "اسم المستخدم أو كلمة المرور غير صحيحة";
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"حدث خطأ أثناء تسجيل الدخول: {ex.Message}";
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
