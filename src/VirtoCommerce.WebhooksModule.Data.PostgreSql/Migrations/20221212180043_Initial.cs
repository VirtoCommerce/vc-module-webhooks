using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.WebhooksModule.Data.PostgreSql.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WebHook",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Url = table.Column<string>(type: "character varying(2083)", maxLength: 2083, nullable: true),
                    ContentType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsAllEvents = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebHook", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebHookFeedEntry",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    WebHookId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    EventId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    AttemptCount = table.Column<int>(type: "integer", nullable: false),
                    RecordType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Error = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    RequestHeaders = table.Column<string>(type: "text", nullable: true),
                    RequestBody = table.Column<string>(type: "text", nullable: true),
                    ResponseHeaders = table.Column<string>(type: "text", nullable: true),
                    ResponseBody = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebHookFeedEntry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebHookEvent",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    EventId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    WebHookId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
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

            migrationBuilder.CreateTable(
                name: "WebHookPayload",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    EventPropertyName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    WebHookId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_WebHookPayload_WebHookId",
                table: "WebHookPayload",
                column: "WebHookId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WebHookEvent");

            migrationBuilder.DropTable(
                name: "WebHookFeedEntry");

            migrationBuilder.DropTable(
                name: "WebHookPayload");

            migrationBuilder.DropTable(
                name: "WebHook");
        }
    }
}
