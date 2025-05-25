using SimpleMongoMigrations;
using SimpleMongoMigrations.Demo.Migrations;
using System.Reflection;

MigrationEngineBuilder
    .Create()
    .WithConnectionString("mongodb://localhost:27017")
    .WithDatabase("TestDB")
    .WithAssembly(Assembly.GetAssembly(typeof(_001_AddDefaultData)))
    .Build()
    .Run();