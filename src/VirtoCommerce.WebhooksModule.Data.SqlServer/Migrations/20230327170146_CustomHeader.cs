using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.WebhooksModule.Data.SqlServer.Migrations
{
    public partial class CustomHeader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomHttpHeaderName",
                table: "WebHook",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomHttpHeaderValue",
                table: "WebHook",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomHttpHeaderName",
                table: "WebHook");

            migrationBuilder.DropColumn(
                name: "CustomHttpHeaderValue",
                table: "WebHook");
        }
    }
}
