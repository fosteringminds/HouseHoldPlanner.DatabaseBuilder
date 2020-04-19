using System;
using Xunit;
using Npgsql;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.IO;
using HouseHoldPlanner.DatabaseBuilder.Models;
using HouseHoldPlanner.DatabaseBuilder.Processor;
using System.Threading.Tasks;
using System.Linq;

namespace HouseHoldPlanner.DatabaseBuilder.Tests
{
    public class DatabaseBuilderTests
    {
        [Fact]
        public async Task InitializeMigrationLogDb_DatabaseExists_Test()
        {
            //need a script to create the database or do we go straight into postgres itself and create the database?

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            DatabaseBuilderSettings databaseBuilderSettings = new DatabaseBuilderSettings();
            IConfiguration config = builder.Build();
            config.GetSection("DatabaseBuilderSettings").Bind(databaseBuilderSettings);

            Assert.True(!string.IsNullOrWhiteSpace(databaseBuilderSettings.SqlSourceDir));
            Assert.True(!string.IsNullOrWhiteSpace(databaseBuilderSettings.MigrationSourceDir));
            Assert.True(!string.IsNullOrWhiteSpace(databaseBuilderSettings.DatabaseHost));
            Assert.True(!string.IsNullOrWhiteSpace(databaseBuilderSettings.DatabaseUserName));
            Assert.True(!string.IsNullOrWhiteSpace(databaseBuilderSettings.DatabasePassword));

            //need a method returning a list of migrations from the migrations directory
            MigrationProcessor migrationProcessor = new MigrationProcessor(databaseBuilderSettings);
            await migrationProcessor.Run();

            Assert.NotNull(migrationProcessor.MigrationLogs);
            Assert.True(migrationProcessor.MigrationLogs.Count > 0);
            Assert.True(migrationProcessor.MigrationLogs.First().MigrationLogId > 0);
        }
    }
}
