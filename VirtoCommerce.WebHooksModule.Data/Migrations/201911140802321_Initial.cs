namespace VirtoCommerce.WebhooksModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WebHook",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(maxLength: 1024),
                        Url = c.String(maxLength: 2083),
                        ContentType = c.String(maxLength: 128),
                        IsActive = c.Boolean(nullable: false),
                        IsAllEvents = c.Boolean(nullable: false),
                        RaisedEventCount = c.Long(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedBy = c.String(maxLength: 64),
                        ModifiedBy = c.String(maxLength: 64),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WebHookEvent",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        EventId = c.String(maxLength: 128),
                        WebHookId = c.String(maxLength: 128),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedBy = c.String(maxLength: 64),
                        ModifiedBy = c.String(maxLength: 64),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WebHook", t => t.WebHookId)
                .Index(t => t.WebHookId);
            
            CreateTable(
                "dbo.WebHookFeedEntry",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        WebHookId = c.String(maxLength: 128),
                        EventId = c.String(maxLength: 128),
                        AttemptCount = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Error = c.String(maxLength: 1024),
                        RequestHeaders = c.String(),
                        RequestBody = c.String(),
                        ResponseHeaders = c.String(),
                        ResponseBody = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedBy = c.String(maxLength: 64),
                        ModifiedBy = c.String(maxLength: 64),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WebHookEvent", "WebHookId", "dbo.WebHook");
            DropIndex("dbo.WebHookEvent", new[] { "WebHookId" });
            DropTable("dbo.WebHookFeedEntry");
            DropTable("dbo.WebHookEvent");
            DropTable("dbo.WebHook");
        }
    }
}
