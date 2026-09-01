using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using KOSS.Web.Models;
using KOSS.Web.Models.ViewModels;

namespace KOSS.Web.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager   _userManager;

        public AccountController() { }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            _userManager   = userManager;
            _signInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager =>
            _signInManager ?? (_signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>());

        public ApplicationUserManager UserManager =>
            _userManager ?? (_userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>());

        // ──────────────────────────────────────────────
        //  GET: /Account/Login
        // ──────────────────────────────────────────────
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // ──────────────────────────────────────────────
        //  POST: /Account/Login
        // ──────────────────────────────────────────────
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid) return View(model);

            using (var db = new KossDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Email == model.Email || u.UserName == model.Email);
                if (user != null && user.IsActive)
                {
                    var hasher = new PasswordHasher();
                    var verify = hasher.VerifyHashedPassword(user.PasswordHash, model.Password);
                    if (verify == PasswordVerificationResult.Success || verify == PasswordVerificationResult.SuccessRehashNeeded)
                    {
                        var identity = await user.GenerateUserIdentityAsync(UserManager);
                        AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = model.RememberMe }, identity);
                        return RedirectToLocal(returnUrl);
                    }
                }
            }

            ModelState.AddModelError("", "البريد الإلكتروني أو كلمة المرور غير صحيحة.");
            return View(model);
        }

        // ──────────────────────────────────────────────
        //  POST: /Account/LogOff
        // ──────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        // ──────────────────────────────────────────────
        //  GET: /Account/Register  (للمديرين فقط)
        // ──────────────────────────────────────────────
        [Authorize(Roles = "Executive")]
        public ActionResult Register()
        {
            ViewBag.Roles = new SelectList(new[]
            {
                new { Value = "SalesStaff",     Text = "موظف مبيعات" },
                new { Value = "Designer",       Text = "مصمم داخلي" },
                new { Value = "FieldSurveyor",  Text = "مساح ميداني" },
                new { Value = "Finance",        Text = "مسؤول مالي" },
                new { Value = "FactoryManager", Text = "مدير مصنع" },
                new { Value = "Executive",      Text = "مدير تنفيذي" },
            }, "Value", "Text");
            return View();
        }

        // ──────────────────────────────────────────────
        //  POST: /Account/Register
        // ──────────────────────────────────────────────
        [HttpPost, Authorize(Roles = "Executive"), ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email    = model.Email,
                FullName = model.FullName
            };

            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await UserManager.AddToRoleAsync(user.Id, model.Role);
                TempData["Success"] = $"تم إنشاء حساب المستخدم '{model.FullName}' بنجاح.";
                return RedirectToAction("Index", "Hr");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);

            return View(model);
        }

        private IAuthenticationManager AuthenticationManager =>
            HttpContext.GetOwinContext().Authentication;

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Index", "Dashboard");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _userManager?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
