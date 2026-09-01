using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using KOSS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KOSS.Web.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // إنشاء الجداول تلقائياً إن لم تكن موجودة
            await context.Database.EnsureCreatedAsync();

            // 1. إنشاء المستخدمين الافتراضيين
            if (!await context.Users.AnyAsync())
            {
                var adminUser = new User
                {
                    Username = "admin@koss.ly",
                    Password = "Admin@123",
                    FullName = "المدير التنفيذي لمنظومة بن سوما",
                    Role = "Executive"
                };

                var salesUser = new User
                {
                    Username = "sales@koss.ly",
                    Password = "User@123",
                    FullName = "مسؤول المبيعات والمعارض",
                    Role = "Sales"
                };

                context.Users.AddRange(adminUser, salesUser);
                await context.SaveChangesAsync();

                // الصلاحيات
                var perms = new List<Permission>
                {
                    new Permission { Name = "ManageRequests", DisplayName = "إدارة طلبات المطابخ", Description = "صلاحية فتح وتعديل ومتابعة طلبات المطابخ" },
                    new Permission { Name = "ManageDesigns", DisplayName = "إدارة التصاميم 3D", Description = "صلاحية رفع واعتماد مخططات الـ 3D" },
                    new Permission { Name = "ManageQuotations", DisplayName = "إدارة عروض الأسعار", Description = "صلاحية تحرير واعتماد عروض الأسعار" },
                    new Permission { Name = "ManageContracts", DisplayName = "إدارة العقود والدفعات", Description = "صلاحية توقيع العقود وقبض الدفعات" },
                    new Permission { Name = "ManageFactory", DisplayName = "إدارة المصنع والتشغيل", Description = "صلاحية متابعة خطوط الإنتاج وفحص الجودة" },
                    new Permission { Name = "ManageInventory", DisplayName = "إدارة المستودعات", Description = "صلاحية الصرف والاستلام المخزني" },
                    new Permission { Name = "ManageAccounting", DisplayName = "الإدارة المالية والتكاليف", Description = "صلاحية استعراض تقارير الأرباح وإغلاق المشاريع" }
                };
                context.Permissions.AddRange(perms);
                await context.SaveChangesAsync();
            }

            // 2. إنشاء الموظفين وفريق العمل
            if (!await context.StaffMembers.AnyAsync())
            {
                var staff = new List<StaffMember>
                {
                    new StaffMember { FullName = "م. أحمد الشريف", Phone = "0912345678", Role = StaffRole.FieldSurveyor, IsActive = true, BaseSalary = 3500 },
                    new StaffMember { FullName = "م. سارة الترهوني", Phone = "0923456789", Role = StaffRole.Designer, IsActive = true, BaseSalary = 4000 },
                    new StaffMember { FullName = "أ. محمود القمودي", Phone = "0934567890", Role = StaffRole.SalesStaff, IsActive = true, BaseSalary = 2800 },
                    new StaffMember { FullName = "م. طارق الزوي", Phone = "0945678901", Role = StaffRole.FactoryManager, IsActive = true, BaseSalary = 4500 }
                };
                context.StaffMembers.AddRange(staff);
                await context.SaveChangesAsync();
            }

            // 3. إنشاء المستودع ودليل الأصناف
            if (!await context.ItemMasters.AnyAsync())
            {
                var wh = new Warehouse
                {
                    Code = "WH-MAIN",
                    Name = "المستودع المركزي - طريق المطار",
                    Location = "طرابلس",
                    KeeperName = "أمين المخزن العام",
                    IsActive = true
                };
                context.Warehouses.Add(wh);
                await context.SaveChangesAsync();

                var items = new List<ItemMaster>
                {
                    new ItemMaster { ItemCode = "WOOD-HDF-01", Name = "ألواح خشب HDF إسباني عالي المقاومة 18 ملم", Category = "أخشاب", Unit = "لوح", StandardCost = 185, StandardSalePrice = 240, ReorderLevel = 15 },
                    new ItemMaster { ItemCode = "WOOD-MEL-02", Name = "ألواح ميلامين أبيض داخلي مقاوم للرطوبة 18 ملم", Category = "أخشاب", Unit = "لوح", StandardCost = 120, StandardSalePrice = 160, ReorderLevel = 20 },
                    new ItemMaster { ItemCode = "HW-BLUM-01", Name = "مفصلات هيدروليك Blum ناعمة الإغلاق (نمساوي)", Category = "إكسسوارات", Unit = "قطعة", StandardCost = 14, StandardSalePrice = 22, ReorderLevel = 50 },
                    new ItemMaster { ItemCode = "HW-BLUM-BOX", Name = "سحابات أدراج مخفية Blum Tandembox مع الفرامل", Category = "إكسسوارات", Unit = "طقم", StandardCost = 85, StandardSalePrice = 130, ReorderLevel = 12 },
                    new ItemMaster { ItemCode = "PVC-EDGE-01", Name = "شريط حواف PVC تركي سماكة 2 ملم", Category = "إكسسوارات", Unit = "متر", StandardCost = 2.5m, StandardSalePrice = 4.5m, ReorderLevel = 100 },
                    new ItemMaster { ItemCode = "TOP-QUARTZ-01", Name = "سطح رخام كوارتز طبيعي إيطالي رمادي كالكاتا", Category = "رخام", Unit = "متر طولي", StandardCost = 240, StandardSalePrice = 340, ReorderLevel = 10 }
                };

                context.ItemMasters.AddRange(items);
                await context.SaveChangesAsync();

                foreach (var itm in items)
                {
                    context.StockItems.Add(new StockItem
                    {
                        WarehouseId = wh.Id,
                        ItemMasterId = itm.Id,
                        PhysicalQuantity = 60,
                        ReservedQuantity = 0,
                        WeightedAverageCost = itm.StandardCost,
                        LastUpdated = DateTime.Now
                    });
                }
                await context.SaveChangesAsync();
            }

            // 4. عملاء افتراضيون وطلب مطبخ استرشادي
            if (!await context.Customers.AnyAsync())
            {
                var cust1 = new Customer
                {
                    Name = "د. كمال المهدي اليعقوبي",
                    Phone = "0918889900",
                    District = "حي الأندلس، طرابلس",
                    Address = "بالقرب من السفارة، فيلا رقم 14",
                    CreatedBy = "admin@koss.ly"
                };
                context.Customers.Add(cust1);
                await context.SaveChangesAsync();

                var req = new KitchenRequest
                {
                    RequestNumber = "REQ-2026-1001",
                    CustomerId = cust1.Id,
                    Location = "حي الأندلس، طرابلس",
                    ProjectType = ProjectType.Villa,
                    LayoutType = KitchenLayoutType.Straight,
                    TargetDeliveryDate = DateTime.Now.AddDays(30),
                    Status = KitchenRequestStatus.ContractActive,
                    Notes = "مطبخ مودرن مع جزيرة وسطية ورخام كوارتز ومفصلات بلوم هيدروليك كاملة",
                    CreatedBy = "admin@koss.ly"
                };
                context.KitchenRequests.Add(req);
                await context.SaveChangesAsync();

                // عقد ومخطط وعرض سعر مرتبط بالطلب
                var contract = new Contract
                {
                    KitchenRequestId = req.Id,
                    ContractNumber = "CNT-2026-5001",
                    SignedDate = DateTime.Now.AddDays(-3),
                    TargetCompletionDate = DateTime.Now.AddDays(27),
                    TotalValue = 18500,
                    RequiredDeposit = 5550,
                    TotalPaid = 5550,
                    TotalMeters = 8.5m,
                    PricePerMeter = 2176.47m,
                    Status = ContractStatus.Active,
                    CreatedBy = "admin@koss.ly",
                    PaymentSchedules = new List<PaymentSchedule>
                    {
                        new PaymentSchedule { StageName = "عربون التعاقد (30%)", Percentage = 30, Amount = 5550, DueDate = DateTime.Now.AddDays(-3), IsPaid = true, PaidAt = DateTime.Now.AddDays(-3) },
                        new PaymentSchedule { StageName = "دفعة بدء التصنيع بالمصنع (40%)", Percentage = 40, Amount = 7400, DueDate = DateTime.Now.AddDays(7), IsPaid = false },
                        new PaymentSchedule { StageName = "دفعة التوريد والتركيب (20%)", Percentage = 20, Amount = 3700, DueDate = DateTime.Now.AddDays(20), IsPaid = false },
                        new PaymentSchedule { StageName = "مخالصة التسليم النهائي (10%)", Percentage = 10, Amount = 1850, DueDate = DateTime.Now.AddDays(27), IsPaid = false }
                    }
                };
                context.Contracts.Add(contract);

                var payment = new Payment
                {
                    ContractId = contract.Id,
                    ReceiptNumber = "REC-2026-9001",
                    Amount = 5550,
                    PaymentType = PaymentType.Deposit,
                    PaymentMethod = PaymentMethod.BankTransfer,
                    ReferenceNumber = "TXN-884920",
                    PaidAt = DateTime.Now.AddDays(-3),
                    ReceivedBy = "المحصل المالي",
                    Notes = "تحويل مصرفي سداد دفعة العربون الأولى 30%"
                };
                context.Payments.Add(payment);

                var wo = new WorkOrder
                {
                    KitchenRequestId = req.Id,
                    ContractId = contract.Id,
                    OrderNumber = "WO-2026-3001",
                    ExpectedEndDate = DateTime.Now.AddDays(20),
                    Status = WorkOrderStatus.Manufacturing,
                    CreatedBy = "admin@koss.ly",
                    Tasks = new List<ManufacturingTask>
                    {
                        new ManufacturingTask { TaskName = "1. قص ألواح HDF والميلامين CNC", Status = "Completed", CompletedAt = DateTime.Now.AddDays(-1) },
                        new ManufacturingTask { TaskName = "2. شريط حواف PVC أوتوماتيكي", Status = "InProgress", StartedAt = DateTime.Now },
                        new ManufacturingTask { TaskName = "3. التجميع الميكانيكي وتركيب المفصلات", Status = "Pending" },
                        new ManufacturingTask { TaskName = "4. فحص الجودة والمطابقة والتغليف", Status = "Pending" }
                    }
                };
                context.WorkOrders.Add(wo);
                await context.SaveChangesAsync();
            }
        }
    }
}
