using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Tests.VerifyMigrationOrder.Helpers;

namespace SimpleMongoMigrations.Tests.VerifyMigrationOrder.Migrations
{
    public class _004_HasNoVersion : IMigration
    {
        public void Up(IMongoDatabase database)
        {
            MongoHelper.AppendTextToAllPersonsData(database, "no version");
        }
    }
}
