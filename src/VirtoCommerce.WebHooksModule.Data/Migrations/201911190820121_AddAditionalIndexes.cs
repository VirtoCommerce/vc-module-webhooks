namespace VirtoCommerce.WebhooksModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAditionalIndexes : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.WebHookEvent", new[] { "WebHookId" });
            AddColumn("dbo.WebHookFeedEntry", "RecordType", c => c.Int(nullable: false));
            CreateIndex("dbo.WebHookEvent", "WebHookId");
            CreateIndex("dbo.WebHookFeedEntry", "WebHookId");
            CreateIndex("dbo.WebHookFeedEntry", new[] { "WebHookId", "RecordType" }, name: "IX_WebHookIdAndRecordType");
            CreateIndex("dbo.WebHookFeedEntry", "EventId");
            DropColumn("dbo.WebHook", "RaisedEventCount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WebHook", "RaisedEventCount", c => c.Long(nullable: false));
            DropIndex("dbo.WebHookFeedEntry", new[] { "EventId" });
            DropIndex("dbo.WebHookFeedEntry", "IX_WebHookIdAndRecordType");
            DropIndex("dbo.WebHookFeedEntry", new[] { "WebHookId" });
            DropIndex("dbo.WebHookEvent", new[] { "WebHookId" });
            DropColumn("dbo.WebHookFeedEntry", "RecordType");
            CreateIndex("dbo.WebHookEvent", "WebHookId");
        }
    }
}
