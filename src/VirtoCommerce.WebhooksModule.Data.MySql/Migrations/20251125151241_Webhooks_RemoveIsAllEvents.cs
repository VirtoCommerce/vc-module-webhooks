using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.WebhooksModule.Data.MySql.Migrations
{
    /// <inheritdoc />
    public partial class Webhooks_RemoveIsAllEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAllEvents",
                table: "WebHook");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAllEvents",
                table: "WebHook",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
