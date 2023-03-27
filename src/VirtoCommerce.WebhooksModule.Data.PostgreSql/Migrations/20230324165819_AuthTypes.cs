using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.WebhooksModule.Data.PostgreSql.Migrations
{
    public partial class AuthTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthType",
                table: "WebHook",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BasicPassword",
                table: "WebHook",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BasicUsername",
                table: "WebHook",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BearerToken",
                table: "WebHook",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthType",
                table: "WebHook");

            migrationBuilder.DropColumn(
                name: "BasicPassword",
                table: "WebHook");

            migrationBuilder.DropColumn(
                name: "BasicUsername",
                table: "WebHook");

            migrationBuilder.DropColumn(
                name: "BearerToken",
                table: "WebHook");
        }
    }
}
