using SimpleMongoMigrations;
using SimpleMongoMigrations.Demo.Migrations;
using System.Reflection;

MigrationEngineBuilder
    .Create()
    .WithConnectionString("mongodb://localhost:27017")
    .WithDatabase("TestDB")
    .WithAssembly(Assembly.GetAssembly(typeof(_1_0_0_AddDefaultData)))
    .WithTransactionScope(TransactionScope.AllMigrations) // Optional, can be omitted if not needed
    .Build()
    .Run();