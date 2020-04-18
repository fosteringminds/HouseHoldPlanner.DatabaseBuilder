using HouseHoldPlanner.DatabaseBuilder.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace HouseHoldPlanner.DatabaseBuilder.Processor
{
    public class MigrationProcessor
    {
        private readonly DatabaseBuilderSettings databaseBuilderSettings;
        private NpgsqlConnection dbConnection;
        private NpgsqlConnectionStringBuilder connectionStringBuilder;
        public MigrationProcessor(DatabaseBuilderSettings databaseBuilderSettings)
        {
            this.databaseBuilderSettings = databaseBuilderSettings;
        }        

        public async Task Run()
        {
            InitializeMigrationLogDbConnection();
            await InitMigrationList();
            await LogMigrationListToMigrationLogDb();
        }

        private void InitializeMigrationLogDbConnection()
        {
            connectionStringBuilder = new NpgsqlConnectionStringBuilder();
            connectionStringBuilder.Username = databaseBuilderSettings.DatabaseUserName;
            connectionStringBuilder.Password = databaseBuilderSettings.DatabasePassword;
            connectionStringBuilder.Host = databaseBuilderSettings.DatabaseHost;
        }

        private async Task InitMigrationList()
        {
            if (!Directory.Exists(databaseBuilderSettings.MigrationSourceDir))
                throw new DirectoryNotFoundException("The migration directory does not exist");

            MigrationLog = new List<MigrationLog>();
            DirectoryInfo di = new DirectoryInfo(databaseBuilderSettings.MigrationSourceDir);
            var fileInfo = di.GetFiles("migration*.json");
            foreach(var fi in fileInfo)
            {
                using(StreamReader reader = fi.OpenText())
                {
                    string json = "";
                    while ((json = await reader.ReadLineAsync()) != null)
                    {
                       MigrationLog.AddRange(JsonConvert.DeserializeObject<IEnumerable<MigrationLog>>(json));
                    }
                }
            }
        }

        private async Task LogMigrationListToMigrationLogDb()
        {
            await InitializeMigrationLogDb();
        }

        private async Task InitializeMigrationLogDb()
        {
            await CreateMigrationLogDb();
            await CreateMigrationLogSchema();
            await CreateMigrationLogTable();
            await SaveMigrationLogs();
        }

        private async Task CreateMigrationLogDb()
        {
            try
            {
                using (dbConnection = new NpgsqlConnection(connectionStringBuilder.ConnectionString))
                {
                    await dbConnection.ExecuteAsync($"CREATE DATABASE {databaseBuilderSettings.MigrationLogDbName};");
                }
            }
            catch(PostgresException pex)
            {
                var expectedMessage = $"42P04: database \"{databaseBuilderSettings.MigrationLogDbName}\" already exists";
                if (pex.Message != expectedMessage)
                    throw pex;
            }        
            catch(Exception ex)
            {
                throw ex;
            }

            connectionStringBuilder.Database = databaseBuilderSettings.MigrationLogDbName;
        }

        private async Task CreateMigrationLogSchema()
        {
            if (!await SchemaExists())
            {
                try
                {
                    using (dbConnection = new NpgsqlConnection(connectionStringBuilder.ConnectionString))
                    {
                        await dbConnection.ExecuteAsync(
                            $@"create schema {databaseBuilderSettings.MigrationLogSchemaName};");
                    }
                }
                catch (PostgresException pex)
                {
                    throw pex;
                }
            }
        }

        private async Task<bool> SchemaExists()
        {
            bool schemaExists = false;

            try
            {
                using (dbConnection = new NpgsqlConnection(connectionStringBuilder.ConnectionString))
                {
                    var q = await dbConnection.QueryFirstOrDefaultAsync<string>("SELECT schema_name FROM information_schema.schemata WHERE schema_name = :MigrationLogSchemaName;", new { databaseBuilderSettings.MigrationLogSchemaName });
                    if (q != null)
                    {
                        schemaExists = true;
                    }
                }
            }
            catch(PostgresException pex)
            {
                throw pex;
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return schemaExists;
        }

        private async Task CreateMigrationLogTable()
        {
            if (!await TableExists())
            {
                try
                {
                    using (dbConnection = new NpgsqlConnection(connectionStringBuilder.ConnectionString))
                    {
                        var commandText = $@"
                        set search_path={databaseBuilderSettings.MigrationLogSchemaName};
                        create table {databaseBuilderSettings.MigrationLogDbTableName} (
                            migration_log_id serial primary key not null,
                            migration_log jsonb not null,
                            created_at timestamptz default current_timestamp);";
                        await dbConnection.ExecuteAsync(commandText);
                    }
                }
                catch (PostgresException pex)
                {
                    throw pex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private async Task<bool> TableExists()
        {
            bool tableExists = false;

            try
            {
                using (dbConnection = new NpgsqlConnection(connectionStringBuilder.ConnectionString))
                {
                    tableExists = await dbConnection.QueryFirstAsync<bool>(
                        @"SELECT EXISTS(
                        SELECT *
                        FROM information_schema.tables
                        WHERE
                          table_schema = :MigrationLogSchemaName AND
                          table_name = :MigrationLogDbTableName
                    );",
                        new { databaseBuilderSettings.MigrationLogSchemaName, databaseBuilderSettings.MigrationLogDbTableName });
                }
            }
            catch (PostgresException pex)
            {
                throw pex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return tableExists;
        }

        private async Task SaveMigrationLogs()
        {
            try
            {
                using (dbConnection = new NpgsqlConnection(connectionStringBuilder.ConnectionString))
                {
                    foreach (var migrationLog in MigrationLog)
                    {
                        var migrationLogJson = JsonConvert.SerializeObject(migrationLog);
                        migrationLog.MigrationLogId = await dbConnection.QueryFirstOrDefaultAsync<long>(
                            $@"
                            set search_path={databaseBuilderSettings.MigrationLogSchemaName};
                            INSERT INTO {databaseBuilderSettings.MigrationLogDbTableName}(migration_log)VALUES(cast(:migrationLogJson as json)) returning migration_log_id;",
                            new { migrationLogJson });
                    }
                }
            }
            catch (PostgresException pex)
            {
                throw pex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MigrationLog> MigrationLog { get; private set; }
    }
}
