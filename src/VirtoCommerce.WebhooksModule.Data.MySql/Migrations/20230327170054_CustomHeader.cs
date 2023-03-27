using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.WebhooksModule.Data.MySql.Migrations
{
    public partial class CustomHeader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomHttpHeaderName",
                table: "WebHook",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CustomHttpHeaderValue",
                table: "WebHook",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
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
