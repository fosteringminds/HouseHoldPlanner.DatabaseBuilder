# HouseHold Planner Database Builder

The HouseHold Planner "system" will consist of multiple databases. Conceptually each service created should have its own database. The scripts for the databases will be kept under source control. Scripts can come in many forms - table creation/amend, schema creation or function create or update. Data changes might also be possible. It is important to keep the scripts in source control. 

Scripts will be run in a migration. It could be viewed as "similar" to EF Core. A discussion on EF migrations is beyond the scope of this document. Migrations will enable the creation of single deployable scripts. So if a migration needs to create 5 new tables, alter 2 other tables and add a new functions a single executable script for the migration will be generated. A single script makes it easy to handover to a DBA. 

The code in this repository contains folders with the following descriptions.

|Folder|Folder               |Description                                                           |
|------|---------------------|----------------------------------------------------------------------|
|src   |                     |                                                                      |
|      |databasebuilder      |.NET Core solution used to generate scripts from migrations.          |
|      |migrations           |Folder containing JSON documents with migration details.              |
|      |sql                  |The SQL referenced by the migration documents. Postgres is the db.    |
|      |test                 |Tests for the databasebuilder.                                        |

## Migration Schema

The migration JSON documents contains two types, MigrationLog and MigrationLogScript.

### MigrationLog

|Property Name           |Property Type       |Description                                                    |
|------------------------|--------------------|---------------------------------------------------------------|
|MigrationLogDatabaseName|string              |The name of the database the migration log is being applied too|
|MigrationLogName        |string              |The name of a migration log                                    |
|MigrationLogId          |int64               |For persistence purposes                                       |
|MigrationLogDate        |DateTime            |The migration log date                                         |
|MigrationLogScripts     |MigrationLogScript[]|A collection of scripts for the particular migration           |

### MigrationLogScript

|Property Name           |Property Type       |Description                                                    |
|------------------------|--------------------|---------------------------------------------------------------|
|MigrationLogScript      |string              |The name of the script to be run                               |