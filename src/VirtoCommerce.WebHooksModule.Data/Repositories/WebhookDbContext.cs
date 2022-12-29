using System.Reflection;
using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.WebhooksModule.Data.Models;

namespace VirtoCommerce.WebhooksModule.Data.Repositories
{
    public class WebhookDbContext : DbContextWithTriggers
    {
        public WebhookDbContext(DbContextOptions<WebhookDbContext> options)
            : base(options)
        {
        }

        protected WebhookDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WebHookEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<WebHookEntity>().ToTable("WebHook");
            modelBuilder.Entity<WebHookEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();

            modelBuilder.Entity<WebHookEventEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<WebHookEventEntity>().ToTable("WebHookEvent");
            modelBuilder.Entity<WebHookEventEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            modelBuilder.Entity<WebHookEventEntity>().HasOne(m => m.WebHook).WithMany(m => m.Events)
                .HasForeignKey(m => m.WebHookId).HasConstraintName("FK_dbo.WebHookEvent_dbo.WebHook_WebHookId").OnDelete(DeleteBehavior.Cascade).IsRequired();
            modelBuilder.Entity<WebHookEventEntity>().HasIndex(i => i.WebHookId);

            modelBuilder.Entity<WebHookPayloadEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<WebHookPayloadEntity>().ToTable("WebHookPayload");
            modelBuilder.Entity<WebHookPayloadEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            modelBuilder.Entity<WebHookPayloadEntity>().HasOne(x => x.WebHook).WithMany(x => x.Payloads)
                .HasForeignKey(x => x.WebHookId).HasConstraintName("FK_dbo.WebHookPayload_dbo.WebHook_WebHookId")
                .OnDelete(DeleteBehavior.Cascade).IsRequired();
            modelBuilder.Entity<WebHookPayloadEntity>().HasIndex(x => x.WebHookId);

            modelBuilder.Entity<WebHookFeedEntryEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<WebHookFeedEntryEntity>().ToTable("WebHookFeedEntry");
            modelBuilder.Entity<WebHookFeedEntryEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            modelBuilder.Entity<WebHookFeedEntryEntity>().HasIndex(x => x.WebHookId);
            modelBuilder.Entity<WebHookFeedEntryEntity>().HasIndex(x => x.EventId);
            modelBuilder.Entity<WebHookFeedEntryEntity>().HasIndex(x => new { x.WebHookId, x.RecordType }).HasDatabaseName("IX_WebHookIdAndRecordType");

            base.OnModelCreating(modelBuilder);

            // Allows configuration for an entity type for different database types.
            // Applies configuration from all <see cref="IEntityTypeConfiguration{TEntity}" in VirtoCommerce.WebhooksModule.Data.XXX project. /> 
            switch (this.Database.ProviderName)
            {
                case "Pomelo.EntityFrameworkCore.MySql":
                    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.WebhooksModule.Data.MySql"));
                    break;
                case "Npgsql.EntityFrameworkCore.PostgreSQL":
                    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.WebhooksModule.Data.PostgreSql"));
                    break;
                case "Microsoft.EntityFrameworkCore.SqlServer":
                    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.WebhooksModule.Data.SqlServer"));
                    break;
            }
        }
    }
}
