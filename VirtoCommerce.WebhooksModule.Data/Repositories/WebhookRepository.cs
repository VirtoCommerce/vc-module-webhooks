using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;
using VirtoCommerce.WebhooksModule.Data.Models;

namespace VirtoCommerce.WebhooksModule.Data.Repositories
{
    public class WebHookRepository : EFRepositoryBase, IWebHookRepository
    {
        public WebHookRepository()
        {
        }

        public WebHookRepository(string nameOrConnectionString, params IInterceptor[] interceptors) : base(nameOrConnectionString, null, interceptors)
        {
            Database.SetInitializer<WebHookRepository>(null);
        }

        public WebHookRepository(DbConnection existingConnection, IInterceptor[] interceptors = null) : base(existingConnection, null, interceptors)
        {
        }

        public IQueryable<WebHookEntity> WebHooks => GetAsQueryable<WebHookEntity>();
        public IQueryable<WebHookEventEntity> WebHookEvents => GetAsQueryable<WebHookEventEntity>();
        public IQueryable<WebHookFeedEntryEntity> WebHookFeedEntries => GetAsQueryable<WebHookFeedEntryEntity>();
        public WebHookEntity[] GetWebHooksByIds(string[] ids)
        {
            return WebHooks
                .Where(x => ids.Contains(x.Id))
                .Include(x => x.Events)
                .ToArray();
        }

        public void DeleteWebHooksByIds(string[] ids)
        {
            if (!ids.IsNullOrEmpty())
            {
                const string commandTemplate = @"DELETE FROM WebHook WHERE Id IN ({0})";

                const int batchSize = 500;
                var skip = 0;

                do
                {
                    var batchIds = ids.Skip(skip).Take(batchSize).ToArray();
                    ExecuteStoreCommand(commandTemplate, batchIds);

                    skip += batchSize;
                }
                while (skip < ids.Length);
            }
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WebHookEntity>().ToTable("WebHook").HasKey(x => x.Id).Property(x => x.Id);

            modelBuilder.Entity<WebHookEventEntity>().ToTable("WebHookEvent").HasKey(x => x.Id).Property(x => x.Id);
            modelBuilder.Entity<WebHookEventEntity>().HasOptional(m => m.WebHook).WithMany(x => x.Events).HasForeignKey(x => x.WebHookId).WillCascadeOnDelete(false);

            modelBuilder.Entity<WebHookFeedEntryEntity>().ToTable("WebHookFeedEntry").HasKey(x => x.Id).Property(x => x.Id);

            base.OnModelCreating(modelBuilder);
        }

        protected virtual void ExecuteStoreCommand(string commandTemplate, IEnumerable<string> parameterValues)
        {
            var command = CreateCommand(commandTemplate, parameterValues);
            ObjectContext.ExecuteStoreCommand(command.Text, command.Parameters.ToArray());
        }

        protected virtual Command CreateCommand(string commandTemplate, IEnumerable<string> parameterValues)
        {
            var parameters = parameterValues.Select((v, i) => new SqlParameter($"@p{i}", v)).ToArray();
            var parameterNames = string.Join(",", parameters.Select(p => p.ParameterName));

            return new Command
            {
                Text = string.Format(commandTemplate, parameterNames),
                Parameters = parameters.OfType<object>().ToList(),
            };
        }

        protected SqlParameter[] AddArrayParameters<T>(Command cmd, string paramNameRoot, IEnumerable<T> values)
        {
            /* An array cannot be simply added as a parameter to a SqlCommand so we need to loop through things and add it manually. 
             * Each item in the array will end up being it's own SqlParameter so the return value for this must be used as part of the
             * IN statement in the CommandText.
             */
            var parameters = new List<SqlParameter>();
            var parameterNames = new List<string>();
            var paramNbr = 1;
            foreach (var value in values)
            {
                var paramName = string.Format("{0}{1}", paramNameRoot, paramNbr++);
                parameterNames.Add(paramName);
                var p = new SqlParameter(paramName, value);
                cmd.Parameters.Add(p);
                parameters.Add(p);
            }
            cmd.Text = cmd.Text.Replace(paramNameRoot, string.Join(",", parameterNames));

            return parameters.ToArray();
        }
        protected class Command
        {
            public string Text { get; set; }
            public IList<object> Parameters { get; set; } = new List<object>();
        }
    }
}