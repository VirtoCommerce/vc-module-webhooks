using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.WebhooksModule.Data.Migrations
{
    public partial class WebHookPayload : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WebHookPayload",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    EventPropertyName = table.Column<string>(maxLength: 128, nullable: true),
                    WebHookId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebHookPayload", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.WebHookPayload_dbo.WebHook_WebHookId",
                        column: x => x.WebHookId,
                        principalTable: "WebHook",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WebHookPayload_WebHookId",
                table: "WebHookPayload",
                column: "WebHookId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WebHookPayload");
        }
    }
}
