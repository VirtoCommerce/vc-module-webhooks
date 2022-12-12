using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.WebhooksModule.Data.Migrations
{
    public partial class UpdateWebHooksV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__MigrationHistory'))
                IF (EXISTS (SELECT * FROM __MigrationHistory WHERE ContextKey = 'VirtoCommerce.WebhooksModule.Data.Migrations.Configuration'))
                    BEGIN
                        BEGIN
	                        INSERT INTO [__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20201201101415_InitialWebHook', '3.1.8')
                        END
                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
