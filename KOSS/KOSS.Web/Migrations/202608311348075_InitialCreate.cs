namespace KOSS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TableName = c.String(maxLength: 100),
                        RecordId = c.Int(nullable: false),
                        Action = c.String(maxLength: 20),
                        OldValue = c.String(),
                        NewValue = c.String(),
                        Description = c.String(maxLength: 300),
                        ChangedBy = c.String(maxLength: 200),
                        ChangedAt = c.DateTime(nullable: false),
                        IpAddress = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BomItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        KitchenUnitId = c.Int(nullable: false),
                        PurchaseOrderId = c.Int(),
                        ItemCode = c.Int(nullable: false),
                        ItemName = c.String(nullable: false, maxLength: 200),
                        Category = c.String(maxLength: 100),
                        Unit = c.String(maxLength: 30),
                        QuantityRequired = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuantityIssued = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitCost = c.Decimal(nullable: false, precision: 18, scale: 3),
                        IssuedToFactory = c.Boolean(nullable: false),
                        Notes = c.String(maxLength: 300),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.KitchenUnits", t => t.KitchenUnitId, cascadeDelete: true)
                .ForeignKey("dbo.PurchaseOrders", t => t.PurchaseOrderId)
                .Index(t => t.KitchenUnitId)
                .Index(t => t.PurchaseOrderId);
            
            CreateTable(
                "dbo.KitchenUnits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContractId = c.Int(nullable: false),
                        UnitType = c.Int(nullable: false),
                        Description = c.String(maxLength: 200),
                        EstimatedValue = c.Decimal(nullable: false, precision: 18, scale: 3),
                        RequiredDepositPercentage = c.Decimal(nullable: false, precision: 5, scale: 2),
                        AllocatedDeposit = c.Decimal(nullable: false, precision: 18, scale: 3),
                        ManufacturingStatus = c.Int(nullable: false),
                        Priority = c.Int(nullable: false),
                        TotalArea = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LengthCm = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WidthCm = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HeightCm = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DesignFilePath = c.String(),
                        DesignedAt = c.DateTime(),
                        DesignedBy = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contracts", t => t.ContractId, cascadeDelete: true)
                .Index(t => t.ContractId);
            
            CreateTable(
                "dbo.Contracts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContractNumber = c.String(),
                        ClientId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        TotalValue = c.Decimal(nullable: false, precision: 18, scale: 3),
                        TotalPaid = c.Decimal(nullable: false, precision: 18, scale: 3),
                        PricePerMeter = c.Decimal(nullable: false, precision: 18, scale: 3),
                        TotalMeters = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Notes = c.String(maxLength: 500),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 150),
                        Phone = c.String(nullable: false, maxLength: 20),
                        Phone2 = c.String(maxLength: 20),
                        Address = c.String(maxLength: 300),
                        District = c.String(maxLength: 100),
                        Status = c.Int(nullable: false),
                        Notes = c.String(maxLength: 500),
                        CreatedAt = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DesignFees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContractId = c.Int(nullable: false),
                        UnitCount = c.Int(nullable: false),
                        FeeAmount = c.Decimal(nullable: false, precision: 18, scale: 3),
                        IsPaid = c.Boolean(nullable: false),
                        ReceiptNumber = c.String(),
                        PaidAt = c.DateTime(),
                        DeductedFromFinalInvoice = c.Boolean(nullable: false),
                        ReceivedBy = c.String(),
                        Notes = c.String(maxLength: 300),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contracts", t => t.ContractId)
                .Index(t => t.ContractId);
            
            CreateTable(
                "dbo.Invoices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContractId = c.Int(nullable: false),
                        InvoiceNumber = c.String(),
                        PricePerMeter = c.Decimal(nullable: false, precision: 18, scale: 3),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 3),
                        Discount = c.Decimal(nullable: false, precision: 18, scale: 3),
                        DesignFeeDeduction = c.Decimal(nullable: false, precision: 18, scale: 3),
                        InvoiceDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        Notes = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contracts", t => t.ContractId)
                .Index(t => t.ContractId);
            
            CreateTable(
                "dbo.InvoiceItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InvoiceId = c.Int(nullable: false),
                        ItemName = c.String(maxLength: 200),
                        Unit = c.String(maxLength: 30),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 3),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Invoices", t => t.InvoiceId, cascadeDelete: true)
                .Index(t => t.InvoiceId);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContractId = c.Int(nullable: false),
                        ReceiptNumber = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 3),
                        PaymentType = c.Int(nullable: false),
                        PaymentMethod = c.Int(nullable: false),
                        ReferenceNumber = c.String(maxLength: 100),
                        PaidAt = c.DateTime(nullable: false),
                        ReceivedBy = c.String(),
                        Notes = c.String(maxLength: 300),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contracts", t => t.ContractId)
                .Index(t => t.ContractId);
            
            CreateTable(
                "dbo.PurchaseOrders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContractId = c.Int(nullable: false),
                        PoNumber = c.String(),
                        Status = c.Int(nullable: false),
                        TotalEstimatedCost = c.Decimal(nullable: false, precision: 18, scale: 3),
                        CreatedAt = c.DateTime(nullable: false),
                        SentToWarehouseAt = c.DateTime(),
                        SentToAccountingAt = c.DateTime(),
                        IssuedToFactoryAt = c.DateTime(),
                        CreatedBy = c.String(),
                        Notes = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contracts", t => t.ContractId)
                .Index(t => t.ContractId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.StaffMembers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        FullName = c.String(nullable: false, maxLength: 150),
                        Role = c.Int(nullable: false),
                        BaseSalary = c.Decimal(nullable: false, precision: 18, scale: 3),
                        BonusBalance = c.Decimal(nullable: false, precision: 18, scale: 3),
                        LiabilityBalance = c.Decimal(nullable: false, precision: 18, scale: 3),
                        JoinDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Notes = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FullName = c.String(),
                        ArabicRole = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.BomItems", "PurchaseOrderId", "dbo.PurchaseOrders");
            DropForeignKey("dbo.BomItems", "KitchenUnitId", "dbo.KitchenUnits");
            DropForeignKey("dbo.KitchenUnits", "ContractId", "dbo.Contracts");
            DropForeignKey("dbo.PurchaseOrders", "ContractId", "dbo.Contracts");
            DropForeignKey("dbo.Payments", "ContractId", "dbo.Contracts");
            DropForeignKey("dbo.InvoiceItems", "InvoiceId", "dbo.Invoices");
            DropForeignKey("dbo.Invoices", "ContractId", "dbo.Contracts");
            DropForeignKey("dbo.DesignFees", "ContractId", "dbo.Contracts");
            DropForeignKey("dbo.Contracts", "ClientId", "dbo.Clients");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.PurchaseOrders", new[] { "ContractId" });
            DropIndex("dbo.Payments", new[] { "ContractId" });
            DropIndex("dbo.InvoiceItems", new[] { "InvoiceId" });
            DropIndex("dbo.Invoices", new[] { "ContractId" });
            DropIndex("dbo.DesignFees", new[] { "ContractId" });
            DropIndex("dbo.Contracts", new[] { "ClientId" });
            DropIndex("dbo.KitchenUnits", new[] { "ContractId" });
            DropIndex("dbo.BomItems", new[] { "PurchaseOrderId" });
            DropIndex("dbo.BomItems", new[] { "KitchenUnitId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.StaffMembers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.PurchaseOrders");
            DropTable("dbo.Payments");
            DropTable("dbo.InvoiceItems");
            DropTable("dbo.Invoices");
            DropTable("dbo.DesignFees");
            DropTable("dbo.Clients");
            DropTable("dbo.Contracts");
            DropTable("dbo.KitchenUnits");
            DropTable("dbo.BomItems");
            DropTable("dbo.AuditLogs");
        }
    }
}
