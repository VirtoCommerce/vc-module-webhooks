using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.WebhooksModule.Data.Migrations
{
    public partial class InitialWebHook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WebHook",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 1024, nullable: true),
                    Url = table.Column<string>(maxLength: 2083, nullable: true),
                    ContentType = table.Column<string>(maxLength: 128, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsAllEvents = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebHook", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebHookFeedEntry",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    WebHookId = table.Column<string>(maxLength: 128, nullable: true),
                    EventId = table.Column<string>(maxLength: 128, nullable: true),
                    AttemptCount = table.Column<int>(nullable: false),
                    RecordType = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Error = table.Column<string>(maxLength: 1024, nullable: true),
                    RequestHeaders = table.Column<string>(nullable: true),
                    RequestBody = table.Column<string>(nullable: true),
                    ResponseHeaders = table.Column<string>(nullable: true),
                    ResponseBody = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebHookFeedEntry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebHookEvent",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    EventId = table.Column<string>(maxLength: 128, nullable: true),
                    WebHookId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebHookEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.WebHookEvent_dbo.WebHook_WebHookId",
                        column: x => x.WebHookId,
                        principalTable: "WebHook",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WebHookEvent_WebHookId",
                table: "WebHookEvent",
                column: "WebHookId");

            migrationBuilder.CreateIndex(
                name: "IX_WebHookFeedEntry_EventId",
                table: "WebHookFeedEntry",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_WebHookFeedEntry_WebHookId",
                table: "WebHookFeedEntry",
                column: "WebHookId");

            migrationBuilder.CreateIndex(
                name: "IX_WebHookIdAndRecordType",
                table: "WebHookFeedEntry",
                columns: new[] { "WebHookId", "RecordType" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WebHookEvent");

            migrationBuilder.DropTable(
                name: "WebHookFeedEntry");

            migrationBuilder.DropTable(
                name: "WebHook");
        }
    }
}
