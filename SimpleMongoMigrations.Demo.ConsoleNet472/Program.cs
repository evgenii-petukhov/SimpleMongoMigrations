using SimpleMongoMigrations.Demo.Migrations;
using System.Reflection;
using System.Threading.Tasks;

namespace SimpleMongoMigrations.Demo.ConsoleNet472
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await MigrationEngineBuilder
                .Create()
                .WithConnectionString("mongodb://localhost:27017") // connection string
                .WithDatabase("TestDB") // database name
                .WithAssembly(Assembly.GetAssembly(typeof(_1_0_0_AddIndexByName))) // assembly to scan for migrations
                .WithTransactionScope(TransactionScope.SingleTransaction) // Optional, can be omitted if not needed
                .Build()
                .RunAsync(default);
        }
    }
}
