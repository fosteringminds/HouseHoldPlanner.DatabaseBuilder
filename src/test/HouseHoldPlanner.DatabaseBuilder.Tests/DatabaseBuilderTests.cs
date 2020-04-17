using System;
using Xunit;
using Npgsql;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.IO;
using HouseHoldPlanner.DatabaseBuilder.Models;
using HouseHoldPlanner.DatabaseBuilder.Processor;

namespace HouseHoldPlanner.DatabaseBuilder.Tests
{
    public class DatabaseBuilderTests
    {
        [Fact]
        public void CreateMigration_Test()
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
            migrationProcessor.Run();

            Assert.NotNull(migrationProcessor.MigrationLog);
            Assert.True(migrationProcessor.MigrationLog.Count > 0);

            /*
            NpgsqlConnectionStringBuilder connectionStringBuilder = new NpgsqlConnectionStringBuilder();
            connectionStringBuilder.Username = "postgres";
            connectionStringBuilder.Password = "postgres";
            connectionStringBuilder.Host = "localhost";

            Assert.NotNull(connectionStringBuilder.ConnectionString);

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionStringBuilder.ConnectionString))
            {
                conn.Execute("CREATE DATABASE databasebuilder;");
            }
            */
        }
    }
}
