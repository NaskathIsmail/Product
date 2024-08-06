namespace ProductList.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate1 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Products", newName: "Items");
            DropColumn("dbo.Items", "IsActive");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Items", "IsActive", c => c.Boolean(nullable: false));
            RenameTable(name: "dbo.Items", newName: "Products");
        }
    }
}
