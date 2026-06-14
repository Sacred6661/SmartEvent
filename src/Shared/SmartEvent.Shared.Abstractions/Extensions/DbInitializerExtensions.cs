using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEvent.Shared.Abstractions.Extensions
{
    public static class DbInitializerExtensions
    {
        public static async Task InitDbAsync<T>(IServiceProvider services) where T : DbContext
        {
            using var scope = services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<T>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(nameof(DbInitializerExtensions));

            await MigrateDbContextAsync(dbContext, logger);
            await dbContext.SaveChangesAsync();
        }

        public static async Task DropDatabasesAsync<T>(IServiceProvider services) where T : DbContext
        {
            using var scope = services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<T>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(nameof(DbInitializerExtensions));

            await DropDbContextAsync(dbContext, logger);
        }


        private static async Task MigrateDbContextAsync(DbContext dbContext, ILogger logger)
        {
            logger.LogInformation("Applying migrations...");
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Migrations applied successfully.");
        }

        private static async Task DropDbContextAsync(DbContext dbContext, ILogger logger)
        {
            logger.LogInformation("Dropping database...");
            await dbContext.Database.EnsureDeletedAsync();
            logger.LogInformation("Database dropped.");
        }
    }
}
