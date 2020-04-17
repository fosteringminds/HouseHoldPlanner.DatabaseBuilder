using HouseHoldPlanner.DatabaseBuilder.Models;
using System;

namespace HouseHoldPlanner.DatabaseBuilder.Processor
{
    public class MigrationProcessor
    {
        private readonly DatabaseBuilderSettings databaseBuilderSettings;
        public MigrationProcessor(DatabaseBuilderSettings databaseBuilderSettings)
        {
            this.databaseBuilderSettings = databaseBuilderSettings;
        }        
    }
}
