using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using SimpleMongoMigrations.Tests.VerifyMigrationOrder.Helpers;

namespace SimpleMongoMigrations.Tests.VerifyMigrationOrder.Migrations
{
    [Version("2.0.0")]
    [Name("Adds age")]
    public class _002_AddAge : IMigration
    {
        public void Up(IMongoDatabase database)
        {
            MongoHelper.AppendTextToAllPersonsData(database, "28 y.o., ");
        }
    }
}
