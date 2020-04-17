using System;

namespace HouseHoldPlanner.DatabaseBuilder.Models
{
    public class DatabaseBuilderSettings
    {
        public string SqlSourceDir { get; set; }
        public string MigrationSourceDir { get; set; }
        public string MigrationLogDbName { get; set; }
        public string MigrationLogSchemaName { get; set; }
        public string MigrationLogDbTableName { get; set; }
        public string DatabaseUserName { get; set; }
        public string DatabasePassword { get; set; }
        public string DatabaseHost { get; set; }
    }
}
