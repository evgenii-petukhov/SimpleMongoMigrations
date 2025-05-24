using SimpleMongoMigrations.Demo.Migrations;
using System.Reflection;

namespace SimpleMongoMigrations.Demo.ConsoleNet472
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new MigrationEngine(
                "mongodb://localhost:27017",
                "TestDB",
                Assembly.GetAssembly(typeof(_001_AddDefaultData)))
                .Run();
        }
    }
}
