namespace Key_onboarding.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Address = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductSolds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        StoreId = c.Int(nullable: false),
                        DateSold = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Stores", t => t.StoreId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.CustomerId)
                .Index(t => t.StoreId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Stores",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Address = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductSolds", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.ProductSolds", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductSolds", "CustomerId", "dbo.Customers");
            DropIndex("dbo.ProductSolds", new[] { "StoreId" });
            DropIndex("dbo.ProductSolds", new[] { "CustomerId" });
            DropIndex("dbo.ProductSolds", new[] { "ProductId" });
            DropTable("dbo.Stores");
            DropTable("dbo.Products");
            DropTable("dbo.ProductSolds");
            DropTable("dbo.Customers");
        }
    }
}
