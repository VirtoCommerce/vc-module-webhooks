namespace VirtoCommerce.WebhooksModule.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddAditionalIndexes : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.WebHookFeedEntry", "WebHookId");
            CreateIndex("dbo.WebHookFeedEntry", new[] { "WebHookId", "Status" }, name: "IX_WebHookIdAndStatus");
            DropColumn("dbo.WebHook", "RaisedEventCount");
        }

        public override void Down()
        {
            AddColumn("dbo.WebHook", "RaisedEventCount", c => c.Long(nullable: false));
            DropIndex("dbo.WebHookFeedEntry", "IX_WebHookIdAndStatus");
            DropIndex("dbo.WebHookFeedEntry", new[] { "WebHookId" });
        }
    }
}
