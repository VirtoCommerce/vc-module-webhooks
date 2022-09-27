using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.WebhooksModule.Data.Models;

namespace VirtoCommerce.WebhooksModule.Data.Repositories
{
    public class WebHookRepository : DbContextRepositoryBase<WebhookDbContext>, IWebHookRepository
    {
        private readonly int _batchSize = 50;

        public WebHookRepository(WebhookDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<WebHookEntity> WebHooks => DbContext.Set<WebHookEntity>();
        public IQueryable<WebHookEventEntity> WebHookEvents => DbContext.Set<WebHookEventEntity>();
        public IQueryable<WebHookPayloadEntity> WebHookPayloads => DbContext.Set<WebHookPayloadEntity>();
        public IQueryable<WebHookFeedEntryEntity> WebHookFeedEntries => DbContext.Set<WebHookFeedEntryEntity>();

        public Task<WebHookEntity[]> GetWebHooksByIdsAsync(string[] ids) => WebHooks
                .Where(x => ids.Contains(x.Id))
                .Include(x => x.Events)
                .Include(x => x.Payloads)
                .ToArrayAsync();

        public async Task DeleteWebHooksByIdsAsync(string[] ids)
        {
            var webHooks = await GetWebHooksByIdsAsync(ids);
            foreach (var webHook in webHooks)
            {
                Remove(webHook);
            }
        }

        public Task<WebHookFeedEntryEntity[]> GetWebHookFeedEntriesByIdsAsync(string[] ids) => WebHookFeedEntries
                .Where(x => ids.Contains(x.Id))
                .ToArrayAsync();

        public async Task DeleteWebHookFeedEntriesByIdsAsync(string[] ids)
        {
            if (!ids.IsNullOrEmpty())
            {
                var skip = 0;
                do
                {
                    var batchEntries = ids.Skip(skip).Take(_batchSize);

                    await ExecuteSqlCommandAsync("DELETE FROM WebHookFeedEntry WHERE Id IN ({0})", batchEntries);

                    skip += _batchSize;
                }
                while (skip < ids.Length);
            }
        }

        protected virtual Task ExecuteSqlCommandAsync(string commandTemplate, IEnumerable<string> parameterValues)
        {
            if (parameterValues?.Count() > 0)
            {
                var command = CreateCommand(commandTemplate, parameterValues);
                return DbContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters);
            }
            return Task.CompletedTask;
        }


        protected virtual Command CreateCommand(string commandTemplate, IEnumerable<string> parameterValues)
        {
            var parameters = parameterValues.Select((v, i) => new SqlParameter($"@p{i}", v));
            var parameterNames = string.Join(",", parameters.Select(p => p.ParameterName));

            return new Command
            {
                Text = string.Format(commandTemplate, parameterNames),
                Parameters = parameters.OfType<object>(),
            };
        }

        protected class Command
        {
            public string Text { get; set; }
            public IEnumerable<object> Parameters { get; set; }
        }

        public async Task UpdateAttemptCountsAsync(WebHookFeedEntryEntity[] webHookFeedEntries)
        {
            if (!webHookFeedEntries.IsNullOrEmpty())
            {
                var skip = 0;
                do
                {
                    var batchEntries = webHookFeedEntries.Skip(skip).Take(_batchSize);
                    var sb = new StringBuilder();
                    var sqlParameters = new List<SqlParameter>();
                    var i = 0;
                    foreach (var entry in batchEntries)
                    {
                        sb.AppendLine($"UPDATE WebHookFeedEntry SET AttemptCount = @count{i} WHERE Id = @id{i}; ");
                        sqlParameters.Add(new SqlParameter($"@count{i}", entry.AttemptCount));
                        sqlParameters.Add(new SqlParameter($"@id{i}", entry.Id));
                        i++;
                    }

                    await DbContext.Database.ExecuteSqlRawAsync(sb.ToString(), sqlParameters.ToArray());

                    skip += _batchSize;
                }
                while (skip < webHookFeedEntries.Length);
            }
        }
    }
}
