using System;
using Xunit;
using Npgsql;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.IO;
using HouseHoldPlanner.DatabaseBuilder.Models;

namespace HouseHoldPlanner.DatabaseBuilder.Tests
{
    public class DatabaseBuilderTests
    {
        [Fact]
        public void CreateDatabase_Test()
        {
            //need a script to create the database or do we go straight into postgres itself and create the database?

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            DatabaseBuilderSettings databaseBuilderSettings = new DatabaseBuilderSettings();
            IConfiguration config = builder.Build();
            config.GetSection("DatabaseBuilderSettings").Bind(databaseBuilderSettings);

            Assert.NotNull(databaseBuilderSettings);
            Assert.NotNull(databaseBuilderSettings.SqlSourceDir);
            Assert.True(databaseBuilderSettings.SqlSourceDir.Length > 0);

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
