using SimpleMongoMigrations.Demo.Migrations;
using System.Reflection;

namespace SimpleMongoMigrations.Demo.ConsoleNet472
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MigrationEngineBuilder
                .Create()
                .WithConnectionString("mongodb://localhost:27017") // connection string
                .WithDatabase("TestDB") // database name
                .WithAssembly(Assembly.GetAssembly(typeof(_1_0_0_AddDefaultData))) // assembly to scan for migrations
                .WithTransactionScope(TransactionScope.AllMigrations) // Optional, can be omitted if not needed
                .Build()
                .Run();
        }
    }
}
