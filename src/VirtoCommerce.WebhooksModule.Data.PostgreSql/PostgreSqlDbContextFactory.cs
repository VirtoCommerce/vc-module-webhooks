using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using VirtoCommerce.WebhooksModule.Data.Repositories;

namespace VirtoCommerce.WebhooksModule.Data.PostgreSql
{
    public class PostgreSqlDbContextFactory : IDesignTimeDbContextFactory<WebhookDbContext>
    {
        public WebhookDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<WebhookDbContext>();
            var connectionString = args.Any() ? args[0] : "User ID = postgres; Password = password; Host = localhost; Port = 5432; Database = virtocommerce3;";

            builder.UseNpgsql(
                connectionString,
                db => db.MigrationsAssembly(typeof(PostgreSqlDbContextFactory).Assembly.GetName().Name));

            return new WebhookDbContext(builder.Options);
        }
    }
}
