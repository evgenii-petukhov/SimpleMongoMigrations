using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using SimpleMongoMigrations.Tests.VerifyMigrationOrder.Helpers;

namespace SimpleMongoMigrations.Tests.VerifyMigrationOrder.Migrations
{
    [Version(6)]
    [Name("Adds preferences")]
    public class _6_0_0_VersionAsInteger : IMigration
    {
        public void Up(IMongoDatabase database, IClientSessionHandle session)
        {
            MongoHelper.AppendTextToAllPersonsData(database, "loves cars, ");
        }
    }
}
