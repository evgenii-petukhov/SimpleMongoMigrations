using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using SimpleMongoMigrations.Tests.VerifyMigrationOrder.Helpers;
using IgnoreAttribute = SimpleMongoMigrations.Attributes.IgnoreAttribute;

namespace SimpleMongoMigrations.Tests.VerifyMigrationOrder.Migrations
{
    [Version(10)]
    [Name("Ignore")]
    [Ignore]
    public class _5_0_0_Ignore : IMigration
    {
        public void Up(IMongoDatabase database, IClientSessionHandle session)
        {
            MongoHelper.AppendTextToAllPersonsData(database, "ignore");
        }
    }
}
