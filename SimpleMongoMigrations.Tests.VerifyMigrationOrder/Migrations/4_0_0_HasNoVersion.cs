using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Tests.VerifyMigrationOrder.Helpers;

namespace SimpleMongoMigrations.Tests.VerifyMigrationOrder.Migrations
{
    public class _4_0_0_HasNoVersion : IMigration
    {
        public void Up(IMongoDatabase database, IClientSessionHandle session)
        {
            MongoHelper.AppendTextToAllPersonsData(database, "no version");
        }
    }
}
