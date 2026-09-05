using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KOSS.Web.Models
{
    public class AppDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor = null)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // ============================================================
        //  مستخدمين وصلاحيات وسجلات التدقيق (Auth & Audit)
        // ============================================================
        public DbSet<User> Users { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        // ============================================================
        //  العملاء والاستفسارات وطلب المطبخ المركزي (Core ERP)
        // ============================================================
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerInquiry> CustomerInquiries { get; set; }
        public DbSet<KitchenRequest> KitchenRequests { get; set; }
        public DbSet<CabinetUnit> CabinetUnits { get; set; }
        public DbSet<RequestStatusHistory> RequestStatusHistories { get; set; }

        // ============================================================
        //  المعاينات والتصاميم وعروض الأسعار (Design & Quotations)
        // ============================================================
        public DbSet<SiteVisit> SiteVisits { get; set; }
        public DbSet<DesignVersion> DesignVersions { get; set; }
        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<QuotationItem> QuotationItems { get; set; }

        // ============================================================
        //  العقود والدفعات (Contracts & Payments)
        // ============================================================
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<PaymentSchedule> PaymentSchedules { get; set; }
        public DbSet<Payment> Payments { get; set; }

        // ============================================================
        //  أوامر التشغيل والـ BOM والمصنع (WorkOrder & Factory)
        // ============================================================
        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<MaterialRequirement> MaterialRequirements { get; set; }
        public DbSet<ManufacturingTask> ManufacturingTasks { get; set; }
        public DbSet<QualityCheck> QualityChecks { get; set; }

        // ============================================================
        //  التركيب ومحاضر التسليم والمصروفات (Installation & Costing)
        // ============================================================
        public DbSet<InstallationOrder> InstallationOrders { get; set; }
        public DbSet<HandoverDocument> HandoverDocuments { get; set; }
        public DbSet<ProjectExpense> ProjectExpenses { get; set; }

        // ============================================================
        //  المخازن والمشتريات والموردين (Inventory & Procurement)
        // ============================================================
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<ItemMaster> ItemMasters { get; set; }
        public DbSet<StockItem> StockItems { get; set; }
        public DbSet<StockTransaction> StockTransactions { get; set; }
        public DbSet<StockIssue> StockIssues { get; set; }
        public DbSet<StockIssueItem> StockIssueItems { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<PurchaseRequestItem> PurchaseRequestItems { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<GoodsReceipt> GoodsReceipts { get; set; }
        public DbSet<GoodsReceiptItem> GoodsReceiptItems { get; set; }
        public DbSet<StaffMember> StaffMembers { get; set; }
        public DbSet<Client> Clients { get; set; }

        // ============================================================
        //  تتبع وتدقيق التغييرات التلقائي (ChangeTracker Audit Logging)
        // ============================================================
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditEntries = OnBeforeSaveChanges();
            var result = await base.SaveChangesAsync(cancellationToken);
            await OnAfterSaveChanges(auditEntries);
            return result;
        }

        public override int SaveChanges()
        {
            var auditEntries = OnBeforeSaveChanges();
            var result = base.SaveChanges();
            OnAfterSaveChanges(auditEntries).GetAwaiter().GetResult();
            return result;
        }

        private List<AuditEntry> OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditEntry(entry);
                auditEntry.TableName = entry.Entity.GetType().Name;
                auditEntry.Username = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";
                auditEntries.Add(auditEntry);

                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = "Create";
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.AuditType = "Delete";
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.ChangedColumns.Add(propertyName);
                                auditEntry.AuditType = "Update";
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }

            foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
            {
                AuditLogs.Add(auditEntry.ToAudit());
            }

            return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
        }

        private Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return Task.CompletedTask;

            foreach (var auditEntry in auditEntries)
            {
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }
                AuditLogs.Add(auditEntry.ToAudit());
            }

            return base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ضبط الدقة الرقمية لجميع الحقول العشرية لتجنب تحذيرات EF Core
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.GetProperties()
                    .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?));

                foreach (var property in properties)
                {
                    property.SetColumnType("decimal(18,3)");
                }
            }
        }
    }
}
