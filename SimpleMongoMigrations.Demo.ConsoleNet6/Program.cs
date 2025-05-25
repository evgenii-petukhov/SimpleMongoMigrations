using SimpleMongoMigrations;
using SimpleMongoMigrations.Demo.Migrations;
using System.Reflection;

new MigrationEngine(
    "mongodb://localhost:27017",
    "TestDB",
    Assembly.GetAssembly(typeof(_001_AddDefaultData)))
    .Run();