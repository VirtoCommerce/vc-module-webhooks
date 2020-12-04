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

            modelBuilder.Entity<WebHookFeedEntryEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<WebHookFeedEntryEntity>().ToTable("WebHookFeedEntry");
            modelBuilder.Entity<WebHookFeedEntryEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            modelBuilder.Entity<WebHookFeedEntryEntity>().HasIndex(x => x.WebHookId);
            modelBuilder.Entity<WebHookFeedEntryEntity>().HasIndex(x => x.EventId);
            modelBuilder.Entity<WebHookFeedEntryEntity>().HasIndex(x => new { x.WebHookId, x.RecordType }).HasName("IX_WebHookIdAndRecordType");

            base.OnModelCreating(modelBuilder);
        }
    }
}
