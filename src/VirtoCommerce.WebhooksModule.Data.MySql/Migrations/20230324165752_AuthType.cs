using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.WebhooksModule.Data.MySql.Migrations
{
    public partial class AuthType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthType",
                table: "WebHook",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BasicPassword",
                table: "WebHook",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "BasicUsername",
                table: "WebHook",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "BearerToken",
                table: "WebHook",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
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
