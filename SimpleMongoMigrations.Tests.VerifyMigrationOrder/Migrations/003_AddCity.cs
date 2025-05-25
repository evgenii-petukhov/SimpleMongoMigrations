using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using SimpleMongoMigrations.Tests.VerifyMigrationOrder.Helpers;

namespace SimpleMongoMigrations.Tests.VerifyMigrationOrder.Migrations
{
    [Version("3")]
    public class _003_AddCity : IMigration
    {
        public void Up(IMongoDatabase database)
        {
            MongoHelper.AppendTextToAllPersonsData(database, "from Paris, ");
        }
    }
}
