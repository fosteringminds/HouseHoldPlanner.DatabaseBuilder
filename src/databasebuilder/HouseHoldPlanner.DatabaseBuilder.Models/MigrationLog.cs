using System;
using System.Collections.Generic;
using System.Text;

namespace HouseHoldPlanner.DatabaseBuilder.Models
{
	public class MigrationLog
	{
		public string MigrationLogDatabaseName { get; set; }
		public string MigrationLogName { get; set; }
		public long MigrationLogId { get; set; }
		public DateTime MigrationLogDate { get; set; }
		public IEnumerable<MigrationLogScripts> MigrationLogScripts { get; set; }
	}
}
