using System;
using System.Collections.Generic;
using System.Text;

namespace HouseHoldPlanner.DatabaseBuilder.Models
{
	public class MigrationLog
	{
		public long MigrationLogId { get; set; }
		public string DatabaseName { get; set; }
		public DateTime MigrationDate { get; set; }
		public string MigrationName { get; set; }
		public IEnumerable<MigrationLogScripts> MigrationScripts { get; set; }
	}
}
