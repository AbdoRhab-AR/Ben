using System;
using System.Collections.Generic;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using KOSS.Web.Models;

namespace KOSS.Web.Database
{
    // ============================================================
    //  مُهيِّئ قاعدة البيانات - يُنشئ الجداول ويضيف بيانات أولية
    //  يُشغَّل تلقائياً عند أول تشغيل للتطبيق
    // ============================================================
    public class KossDbInitializer : CreateDatabaseIfNotExists<KossDbContext>
    {
        protected override void Seed(KossDbContext context)
        {
            // ─────────────────────────────────────────
            //  إنشاء الأدوار
            // ─────────────────────────────────────────
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            string[] roles = {
                "Executive", "Finance", "SalesStaff",
                "Designer", "FieldSurveyor", "FactoryManager"
            };

            foreach (var role in roles)
            {
                if (!roleManager.RoleExists(role))
                    roleManager.Create(new IdentityRole(role));
            }

            // ─────────────────────────────────────────
            //  إنشاء حساب المدير التنفيذي الافتراضي
            // ─────────────────────────────────────────
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            const string adminEmail    = "admin@koss.ly";
            const string adminPassword = "Admin@123";

            if (userManager.FindByEmail(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName    = adminEmail,
                    Email       = adminEmail,
                    FullName    = "مدير النظام",
                    ArabicRole  = "مدير تنفيذي",
                    IsActive    = true,
                    CreatedAt   = DateTime.Now
                };

                var result = userManager.Create(admin, adminPassword);
                if (result.Succeeded)
                    userManager.AddToRole(admin.Id, "Executive");
            }

            // ─────────────────────────────────────────
            //  إضافة سعر المتر الابتدائي
            // ─────────────────────────────────────────
            System.Web.HttpContext.Current?.Application
                .Set("CurrentPricePerMeter", 850m);

            // ─────────────────────────────────────────
            //  إضافة موظفين افتراضيين للنظام (أمثلة)
            // ─────────────────────────────────────────
            if (!context.StaffMembers.AnyAsync().Result)
            {
                context.StaffMembers.AddRange(new List<StaffMember>
                {
                    new StaffMember { FullName = "أحمد محمد",   Role = StaffRole.SalesStaff,     BaseSalary = 1500, IsActive = true, JoinDate = DateTime.Now },
                    new StaffMember { FullName = "مريم الشريف", Role = StaffRole.Designer,        BaseSalary = 2000, IsActive = true, JoinDate = DateTime.Now },
                    new StaffMember { FullName = "علي الزروق",  Role = StaffRole.FieldSurveyor,   BaseSalary = 1800, IsActive = true, JoinDate = DateTime.Now },
                    new StaffMember { FullName = "فاطمة نوري",  Role = StaffRole.Finance,         BaseSalary = 2200, IsActive = true, JoinDate = DateTime.Now },
                    new StaffMember { FullName = "خالد البركي", Role = StaffRole.FactoryManager,  BaseSalary = 2500, IsActive = true, JoinDate = DateTime.Now },
                });
                context.SaveChanges();
            }

            base.Seed(context);
        }
    }
}
