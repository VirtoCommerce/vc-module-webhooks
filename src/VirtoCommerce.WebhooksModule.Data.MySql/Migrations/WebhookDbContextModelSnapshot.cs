﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VirtoCommerce.WebhooksModule.Data.Repositories;

#nullable disable

namespace VirtoCommerce.WebhooksModule.Data.MySql.Migrations
{
    [DbContext(typeof(WebhookDbContext))]
    partial class WebhookDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("VirtoCommerce.WebhooksModule.Data.Models.WebHookEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<int>("AuthType")
                        .HasColumnType("int");

                    b.Property<string>("BasicPassword")
                        .HasColumnType("longtext");

                    b.Property<string>("BasicUsername")
                        .HasColumnType("longtext");

                    b.Property<string>("BearerToken")
                        .HasColumnType("longtext");

                    b.Property<string>("ContentType")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsAllEvents")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .HasMaxLength(1024)
                        .HasColumnType("varchar(1024)");

                    b.Property<string>("Url")
                        .HasMaxLength(2083)
                        .HasColumnType("varchar(2083)");

                    b.HasKey("Id");

                    b.ToTable("WebHook", (string)null);
                });

            modelBuilder.Entity("VirtoCommerce.WebhooksModule.Data.Models.WebHookEventEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("EventId")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("WebHookId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.HasKey("Id");

                    b.HasIndex("WebHookId");

                    b.ToTable("WebHookEvent", (string)null);
                });

            modelBuilder.Entity("VirtoCommerce.WebhooksModule.Data.Models.WebHookFeedEntryEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<int>("AttemptCount")
                        .HasColumnType("int");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Error")
                        .HasMaxLength(1024)
                        .HasColumnType("varchar(1024)");

                    b.Property<string>("EventId")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("RecordType")
                        .HasColumnType("int");

                    b.Property<string>("RequestBody")
                        .HasColumnType("longtext");

                    b.Property<string>("RequestHeaders")
                        .HasColumnType("longtext");

                    b.Property<string>("ResponseBody")
                        .HasColumnType("longtext");

                    b.Property<string>("ResponseHeaders")
                        .HasColumnType("longtext");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("WebHookId")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("WebHookId");

                    b.HasIndex("WebHookId", "RecordType")
                        .HasDatabaseName("IX_WebHookIdAndRecordType");

                    b.ToTable("WebHookFeedEntry", (string)null);
                });

            modelBuilder.Entity("VirtoCommerce.WebhooksModule.Data.Models.WebHookPayloadEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("EventPropertyName")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("WebHookId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)");

                    b.HasKey("Id");

                    b.HasIndex("WebHookId");

                    b.ToTable("WebHookPayload", (string)null);
                });

            modelBuilder.Entity("VirtoCommerce.WebhooksModule.Data.Models.WebHookEventEntity", b =>
                {
                    b.HasOne("VirtoCommerce.WebhooksModule.Data.Models.WebHookEntity", "WebHook")
                        .WithMany("Events")
                        .HasForeignKey("WebHookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_dbo.WebHookEvent_dbo.WebHook_WebHookId");

                    b.Navigation("WebHook");
                });

            modelBuilder.Entity("VirtoCommerce.WebhooksModule.Data.Models.WebHookPayloadEntity", b =>
                {
                    b.HasOne("VirtoCommerce.WebhooksModule.Data.Models.WebHookEntity", "WebHook")
                        .WithMany("Payloads")
                        .HasForeignKey("WebHookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_dbo.WebHookPayload_dbo.WebHook_WebHookId");

                    b.Navigation("WebHook");
                });

            modelBuilder.Entity("VirtoCommerce.WebhooksModule.Data.Models.WebHookEntity", b =>
                {
                    b.Navigation("Events");

                    b.Navigation("Payloads");
                });
#pragma warning restore 612, 618
        }
    }
}
