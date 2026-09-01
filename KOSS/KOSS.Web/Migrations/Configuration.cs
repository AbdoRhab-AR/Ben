using System;
using System.Data.Entity.Migrations;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using KOSS.Web.Models;

namespace KOSS.Web.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<KOSS.Web.Models.KossDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(KOSS.Web.Models.KossDbContext context)
        {
            // ─────────────────────────────────────────
            // 1. إنشاء أدوار النظام (Roles)
            // ─────────────────────────────────────────
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            string[] roles = { "Executive", "Finance", "SalesStaff", "Designer", "FieldSurveyor", "FactoryManager" };
            foreach (var role in roles)
            {
                if (!roleManager.RoleExists(role))
                    roleManager.Create(new IdentityRole(role));
            }

            // ─────────────────────────────────────────
            // 2. إنشاء المستخدم الإداري الافتراضي
            // ─────────────────────────────────────────
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            string adminEmail = "admin@koss.ly";
            var adminUser = userManager.FindByEmail(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "المدير التنفيذي للشركة",
                    ArabicRole = "المدير التنفيذي",
                    EmailConfirmed = true
                };
                userManager.Create(adminUser, "Admin@123");
                userManager.AddToRole(adminUser.Id, "Executive");
            }

            // ─────────────────────────────────────────
            // 3. موظفو الشركة (Staff Members)
            // ─────────────────────────────────────────
            if (!context.StaffMembers.Any())
            {
                context.StaffMembers.AddOrUpdate(s => s.FullName,
                    new StaffMember { FullName = "أحمد التاجوري", Role = StaffRole.SalesStaff, Phone = "0911000001", Email = "sales@koss.ly", BaseSalary = 2500m, JoinDate = DateTime.Now.AddMonths(-6) },
                    new StaffMember { FullName = "م. عمر القرقني", Role = StaffRole.Designer, Phone = "0911000002", Email = "designer@koss.ly", BaseSalary = 3000m, JoinDate = DateTime.Now.AddMonths(-5) },
                    new StaffMember { FullName = "م. خالد الورفلي", Role = StaffRole.FieldSurveyor, Phone = "0911000003", Email = "surveyor@koss.ly", BaseSalary = 2200m, JoinDate = DateTime.Now.AddMonths(-4) },
                    new StaffMember { FullName = "فني طارق الزنتاني", Role = StaffRole.FactoryManager, Phone = "0911000004", Email = "factory@koss.ly", BaseSalary = 2800m, JoinDate = DateTime.Now.AddMonths(-8) },
                    new StaffMember { FullName = "حسين المبروك", Role = StaffRole.Finance, Phone = "0911000005", Email = "finance@koss.ly", BaseSalary = 3200m, JoinDate = DateTime.Now.AddMonths(-12) }
                );
                context.SaveChanges();
            }

            // ─────────────────────────────────────────
            // 4. المستودعات (Warehouses)
            // ─────────────────────────────────────────
            if (!context.Warehouses.Any())
            {
                context.Warehouses.AddOrUpdate(w => w.Code,
                    new Warehouse { Code = "WH-MAIN", Name = "المستودع الرئيسي - طرابلس", Location = "طريق السواني", KeeperName = "فرج المبروك", IsActive = true },
                    new Warehouse { Code = "WH-WOOD", Name = "مستودع الألواح والخامات الخشبية", Location = "المنطقة الصناعية", KeeperName = "سالم الدرسي", IsActive = true },
                    new Warehouse { Code = "WH-ACC", Name = "مستودع الإكسسوارات والمفصلات", Location = "مبنى المصنع", KeeperName = "رمزي الغرياني", IsActive = true }
                );
                context.SaveChanges();
            }

            // ─────────────────────────────────────────
            // 5. الموردون (Suppliers)
            // ─────────────────────────────────────────
            if (!context.Suppliers.Any())
            {
                context.Suppliers.AddOrUpdate(s => s.Code,
                    new Supplier { Code = "SUP-001", Name = "شركة الألواح الألمانية للخشب", Phone = "0915000001", Email = "wood@supplier.ly", Address = "طرابلس", CurrentBalance = 0 },
                    new Supplier { Code = "SUP-002", Name = "وكالة بلوم للمفصلات والسكك (Blum)", Phone = "0915000002", Email = "blum@supplier.ly", Address = "مصراتة", CurrentBalance = 0 },
                    new Supplier { Code = "SUP-003", Name = "مصنع النخبة لأسطح الكوارتز والرخام", Phone = "0915000003", Email = "quartz@supplier.ly", Address = "بنغازي", CurrentBalance = 0 }
                );
                context.SaveChanges();
            }

            // ─────────────────────────────────────────
            // 6. دليل المواد والأصناف القياسي (ItemMaster)
            // ─────────────────────────────────────────
            if (!context.ItemMasters.Any())
            {
                context.ItemMasters.AddOrUpdate(i => i.ItemCode,
                    new ItemMaster { ItemCode = "WOOD-HDF-18", Name = "لوح خشب HDF مقاوم للرطوبة 18مم (2.80×1.22م)", Category = "ألواح خشب", Unit = "لوح", StandardCost = 145m, StandardSalePrice = 220m, ReorderLevel = 10, IsActive = true },
                    new ItemMaster { ItemCode = "WOOD-MDF-UV", Name = "لوح MDF لمعة عالية UV High Gloss (2.80×1.22م)", Category = "ألواح خشب", Unit = "لوح", StandardCost = 190m, StandardSalePrice = 280m, ReorderLevel = 8, IsActive = true },
                    new ItemMaster { ItemCode = "ACC-HINGE-SOFT", Name = "مفصلة بلوم إغلاق هيدروليكي ناعم (Soft Close)", Category = "مفصلات وإكسسوارات", Unit = "زوج", StandardCost = 12m, StandardSalePrice = 25m, ReorderLevel = 50, IsActive = true },
                    new ItemMaster { ItemCode = "ACC-DRAWER-RUNNER", Name = "سكة درج تاندوم بوكس هيدروليك مخفية 50سم", Category = "سكك وأدراج", Unit = "طقم", StandardCost = 65m, StandardSalePrice = 110m, ReorderLevel = 20, IsActive = true },
                    new ItemMaster { ItemCode = "ACC-HANDLE-ALUM", Name = "مقبض بروفايل ألومنيوم مدمج مخفي (Gola Profile)", Category = "مقابض وبروفايل", Unit = "متر", StandardCost = 28m, StandardSalePrice = 45m, ReorderLevel = 15, IsActive = true },
                    new ItemMaster { ItemCode = "EDGE-PVC-2MM", Name = "شريط حواف PVC سماكة 2مم ضد الماء", Category = "شريط حواف", Unit = "متر", StandardCost = 2.5m, StandardSalePrice = 5m, ReorderLevel = 100, IsActive = true },
                    new ItemMaster { ItemCode = "TOP-QUARTZ-CALA", Name = "سطح كوارتز كلكتا أبيض ناصع مع عروق رمادية", Category = "أسطح كوارتز", Unit = "متر", StandardCost = 220m, StandardSalePrice = 350m, ReorderLevel = 5, IsActive = true },
                    new ItemMaster { ItemCode = "SINK-SS-DBL", Name = "حوض غسيل ستانلس ستيل 304 مزدوج تحت الرخام", Category = "أحواض وأجهزة", Unit = "قطعة", StandardCost = 280m, StandardSalePrice = 420m, ReorderLevel = 4, IsActive = true }
                );
                context.SaveChanges();
            }

            // ─────────────────────────────────────────
            // 7. تغذية الأرصدة المخزنية الافتتاحية (StockItems)
            // ─────────────────────────────────────────
            var mainWh = context.Warehouses.FirstOrDefault(w => w.Code == "WH-MAIN");
            if (mainWh != null && !context.StockItems.Any())
            {
                foreach (var item in context.ItemMasters.ToList())
                {
                    context.StockItems.Add(new StockItem
                    {
                        WarehouseId = mainWh.Id,
                        ItemMasterId = item.Id,
                        PhysicalQuantity = 50m,
                        ReservedQuantity = 0,
                        WeightedAverageCost = item.StandardCost,
                        LastUpdated = DateTime.Now
                    });
                }
                context.SaveChanges();
            }

            // ─────────────────────────────────────────
            // 8. عميل افتراضي وطلب مطبخ افتراضي
            // ─────────────────────────────────────────
            if (!context.Customers.Any())
            {
                var customer = new Customer
                {
                    Name = "المهندس عبدالله محمد الفيتوري",
                    Phone = "0912345678",
                    Phone2 = "0923456789",
                    Email = "abdullah@example.ly",
                    District = "حي الأندلس - طرابلس",
                    Address = "بالقرب من السفارة الألمانية",
                    LeadSource = "معرض الشركة",
                    Notes = "فيلا طابقين - مطبخ رئيسي حرف U ومطبخ تحضيري"
                };
                context.Customers.Add(customer);
                context.SaveChanges();
            }
        }
    }
}
