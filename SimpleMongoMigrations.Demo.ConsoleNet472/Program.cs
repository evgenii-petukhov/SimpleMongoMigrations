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
                .WithConnectionString("mongodb://localhost:27017")
                .WithDatabase("TestDB")
                .WithAssembly(Assembly.GetAssembly(typeof(_001_AddDefaultData)))
                .Build()
                .Run();
        }
    }
}
