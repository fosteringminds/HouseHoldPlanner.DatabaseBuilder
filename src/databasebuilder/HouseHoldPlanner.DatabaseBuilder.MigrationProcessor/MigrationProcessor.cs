using HouseHoldPlanner.DatabaseBuilder.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace HouseHoldPlanner.DatabaseBuilder.Processor
{
    public class MigrationProcessor
    {
        private readonly DatabaseBuilderSettings databaseBuilderSettings;
        public MigrationProcessor(DatabaseBuilderSettings databaseBuilderSettings)
        {
            this.databaseBuilderSettings = databaseBuilderSettings;
        }        

        public void Run()
        {
            InitMigrationLog();
        }

        private void InitMigrationLog()
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
                    while ((json = reader.ReadLine()) != null)
                    {
                       MigrationLog.AddRange(JsonConvert.DeserializeObject<IEnumerable<MigrationLog>>(json));
                    }
                }
            }
        }

        public List<MigrationLog> MigrationLog { get; private set; }
    }
}
