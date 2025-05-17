using SimpleMongoMigrations;
using SimpleMongoMigrations.Tests.Migrations;
using System.Reflection;

new MigrationEngine(
    "mongodb://localhost:27017",
    "TestDB",
    Assembly.GetAssembly(typeof(_001_AddDefaultData)))
    .Run();