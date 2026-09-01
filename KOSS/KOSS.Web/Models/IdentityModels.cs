using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace KOSS.Web.Models
{
    // ============================================================
    //  مستخدم النظام (ApplicationUser)
    // ============================================================
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "الاسم الكامل")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Display(Name = "المسمى الوظيفي بالعربي")]
        [StringLength(100)]
        public string ArabicRole { get; set; }

        [Display(Name = "هل الحساب نشط؟")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            userIdentity.AddClaim(new Claim("FullName", FullName ?? ""));
            return userIdentity;
        }
    }

    // ============================================================
    //  سياق قاعدة البيانات الرئيسي (KossDbContext - KOSS_DB)
    // ============================================================
    public class KossDbContext : IdentityDbContext<ApplicationUser>
    {
        public KossDbContext() : base("KossDbContext", throwIfV1Schema: false) { }

        public static KossDbContext Create() => new KossDbContext();

        // 1. العملاء والاستفسارات
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerInquiry> CustomerInquiries { get; set; }
        public DbSet<Client> Clients { get; set; } // للتوافق

        // 2. طلبات المطابخ والمشروع المركزي
        public DbSet<KitchenRequest> KitchenRequests { get; set; }
        public DbSet<RequestStatusHistory> RequestStatusHistories { get; set; }

        // 3. المعاينات والتصاميم
        public DbSet<SiteVisit> SiteVisits { get; set; }
        public DbSet<DesignVersion> DesignVersions { get; set; }

        // 4. عروض الأسعار
        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<QuotationItem> QuotationItems { get; set; }

        // 5. العقود والمالية
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<PaymentSchedule> PaymentSchedules { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<DesignFee> DesignFees { get; set; }
        public DbSet<KitchenUnit> KitchenUnits { get; set; }

        // 6. أمر التنفيذ والتصنيع
        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<MaterialRequirement> MaterialRequirements { get; set; }
        public DbSet<ManufacturingTask> ManufacturingTasks { get; set; }
        public DbSet<QualityCheck> QualityChecks { get; set; }
        public DbSet<SnagItem> SnagItems { get; set; }

        // 7. المخازن والأصناف
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<ItemMaster> ItemMasters { get; set; }
        public DbSet<StockItem> StockItems { get; set; }
        public DbSet<StockIssue> StockIssues { get; set; }
        public DbSet<StockIssueItem> StockIssueItems { get; set; }
        public DbSet<StockTransaction> StockTransactions { get; set; }
        public DbSet<BomItem> BomItems { get; set; }

        // 8. المشتريات والموردون
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<PurchaseRequestItem> PurchaseRequestItems { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<GoodsReceipt> GoodsReceipts { get; set; }
        public DbSet<GoodsReceiptItem> GoodsReceiptItems { get; set; }

        // 9. التركيب والتسليم
        public DbSet<InstallationOrder> InstallationOrders { get; set; }
        public DbSet<HandoverDocument> HandoverDocuments { get; set; }

        // 10. التكاليف والرقابة والموارد البشرية
        public DbSet<ProjectExpense> ProjectExpenses { get; set; }
        public DbSet<StaffMember> StaffMembers { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ─────────────────────────────────────────
            //  العلاقات الأساسية
            // ─────────────────────────────────────────
            modelBuilder.Entity<KitchenRequest>()
                .HasRequired(r => r.Customer)
                .WithMany(c => c.KitchenRequests)
                .HasForeignKey(r => r.CustomerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Contract>()
                .HasRequired(c => c.KitchenRequest)
                .WithMany(r => r.Contracts)
                .HasForeignKey(c => c.KitchenRequestId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WorkOrder>()
                .HasRequired(w => w.KitchenRequest)
                .WithMany(r => r.WorkOrders)
                .HasForeignKey(w => w.KitchenRequestId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<StockIssue>()
                .HasRequired(s => s.KitchenRequest)
                .WithMany()
                .HasForeignKey(s => s.KitchenRequestId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProjectExpense>()
                .HasRequired(e => e.KitchenRequest)
                .WithMany(r => r.Expenses)
                .HasForeignKey(e => e.KitchenRequestId)
                .WillCascadeOnDelete(false);

            // ─────────────────────────────────────────
            //  دقة الأرقام العشرية (Decimal Precisions)
            // ─────────────────────────────────────────
            modelBuilder.Entity<CustomerInquiry>().Property(i => i.EstimatedAreaM2).HasPrecision(18, 2);
            modelBuilder.Entity<CustomerInquiry>().Property(i => i.EstimatedBudget).HasPrecision(18, 3);

            modelBuilder.Entity<SiteVisit>().Property(s => s.WallLength1Cm).HasPrecision(18, 2);
            modelBuilder.Entity<SiteVisit>().Property(s => s.WallLength2Cm).HasPrecision(18, 2);
            modelBuilder.Entity<SiteVisit>().Property(s => s.WallLength3Cm).HasPrecision(18, 2);
            modelBuilder.Entity<SiteVisit>().Property(s => s.CeilingHeightCm).HasPrecision(18, 2);
            modelBuilder.Entity<SiteVisit>().Property(s => s.EstimatedAreaM2).HasPrecision(18, 2);

            modelBuilder.Entity<DesignVersion>().Property(d => d.EstimatedLinearMeters).HasPrecision(18, 2);

            modelBuilder.Entity<Quotation>().Property(q => q.SubTotal).HasPrecision(18, 3);
            modelBuilder.Entity<Quotation>().Property(q => q.Discount).HasPrecision(18, 3);
            modelBuilder.Entity<Quotation>().Property(q => q.TaxAmount).HasPrecision(18, 3);
            modelBuilder.Entity<Quotation>().Property(q => q.TotalAmount).HasPrecision(18, 3);

            modelBuilder.Entity<QuotationItem>().Property(qi => qi.Quantity).HasPrecision(18, 2);
            modelBuilder.Entity<QuotationItem>().Property(qi => qi.UnitPrice).HasPrecision(18, 3);
            modelBuilder.Entity<QuotationItem>().Property(qi => qi.Discount).HasPrecision(18, 3);
            modelBuilder.Entity<QuotationItem>().Property(qi => qi.TotalPrice).HasPrecision(18, 3);

            modelBuilder.Entity<Contract>().Property(c => c.TotalValue).HasPrecision(18, 3);
            modelBuilder.Entity<Contract>().Property(c => c.RequiredDeposit).HasPrecision(18, 3);
            modelBuilder.Entity<Contract>().Property(c => c.TotalPaid).HasPrecision(18, 3);
            modelBuilder.Entity<Contract>().Property(c => c.PricePerMeter).HasPrecision(18, 3);
            modelBuilder.Entity<Contract>().Property(c => c.TotalMeters).HasPrecision(18, 2);
            modelBuilder.Entity<Contract>().Property(c => c.PenaltyPerDay).HasPrecision(18, 3);

            modelBuilder.Entity<PaymentSchedule>().Property(ps => ps.Percentage).HasPrecision(5, 2);
            modelBuilder.Entity<PaymentSchedule>().Property(ps => ps.Amount).HasPrecision(18, 3);

            modelBuilder.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 3);
            modelBuilder.Entity<DesignFee>().Property(d => d.FeeAmount).HasPrecision(18, 3);

            modelBuilder.Entity<MaterialRequirement>().Property(m => m.QuantityRequired).HasPrecision(18, 2);
            modelBuilder.Entity<MaterialRequirement>().Property(m => m.QuantityReserved).HasPrecision(18, 2);
            modelBuilder.Entity<MaterialRequirement>().Property(m => m.QuantityIssued).HasPrecision(18, 2);
            modelBuilder.Entity<MaterialRequirement>().Property(m => m.EstimatedUnitCost).HasPrecision(18, 3);

            modelBuilder.Entity<ItemMaster>().Property(im => im.StandardCost).HasPrecision(18, 3);
            modelBuilder.Entity<ItemMaster>().Property(im => im.StandardSalePrice).HasPrecision(18, 3);
            modelBuilder.Entity<ItemMaster>().Property(im => im.ReorderLevel).HasPrecision(18, 2);

            modelBuilder.Entity<StockItem>().Property(si => si.PhysicalQuantity).HasPrecision(18, 2);
            modelBuilder.Entity<StockItem>().Property(si => si.ReservedQuantity).HasPrecision(18, 2);
            modelBuilder.Entity<StockItem>().Property(si => si.WeightedAverageCost).HasPrecision(18, 3);

            modelBuilder.Entity<StockIssue>().Property(si => si.TotalCost).HasPrecision(18, 3);
            modelBuilder.Entity<StockIssueItem>().Property(sii => sii.QuantityIssued).HasPrecision(18, 2);
            modelBuilder.Entity<StockIssueItem>().Property(sii => sii.UnitCost).HasPrecision(18, 3);

            modelBuilder.Entity<StockTransaction>().Property(st => st.InQuantity).HasPrecision(18, 2);
            modelBuilder.Entity<StockTransaction>().Property(st => st.OutQuantity).HasPrecision(18, 2);
            modelBuilder.Entity<StockTransaction>().Property(st => st.UnitCost).HasPrecision(18, 3);

            modelBuilder.Entity<Supplier>().Property(s => s.CurrentBalance).HasPrecision(18, 3);

            modelBuilder.Entity<PurchaseRequestItem>().Property(pri => pri.QuantityRequested).HasPrecision(18, 2);
            modelBuilder.Entity<PurchaseOrder>().Property(po => po.TotalEstimatedCost).HasPrecision(18, 3);

            modelBuilder.Entity<GoodsReceiptItem>().Property(gri => gri.QuantityReceived).HasPrecision(18, 2);
            modelBuilder.Entity<GoodsReceiptItem>().Property(gri => gri.QuantityDamaged).HasPrecision(18, 2);
            modelBuilder.Entity<GoodsReceiptItem>().Property(gri => gri.UnitCost).HasPrecision(18, 3);

            modelBuilder.Entity<InstallationOrder>().Property(io => io.InstalledLinearMeters).HasPrecision(18, 2);
            modelBuilder.Entity<ProjectExpense>().Property(pe => pe.Amount).HasPrecision(18, 3);

            modelBuilder.Entity<StaffMember>().Property(s => s.BaseSalary).HasPrecision(18, 3);
            modelBuilder.Entity<StaffMember>().Property(s => s.BonusBalance).HasPrecision(18, 3);
            modelBuilder.Entity<StaffMember>().Property(s => s.LiabilityBalance).HasPrecision(18, 3);
        }
    }
}
