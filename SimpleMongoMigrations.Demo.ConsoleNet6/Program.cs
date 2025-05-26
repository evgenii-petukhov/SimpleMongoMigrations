using SimpleMongoMigrations;
using SimpleMongoMigrations.Demo.Migrations;
using System.Reflection;

await MigrationEngineBuilder
    .Create()
    .WithConnectionString("mongodb://localhost:27017") // connection string
    .WithDatabase("TestDB") // database name
    .WithAssembly(Assembly.GetAssembly(typeof(_1_0_0_AddDefaultData))) // assembly to scan for migrations
    .WithTransactionScope(TransactionScope.SingleTransaction) // Optional, can be omitted if not needed
    .Build()
    .RunAsync(default);