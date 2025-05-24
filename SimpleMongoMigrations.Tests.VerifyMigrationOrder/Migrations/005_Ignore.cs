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
    public class _005_Ignore : IMigration
    {
        public void Up(IMongoDatabase database)
        {
            MongoHelper.AppendTextToAllPersonsData(database, "ignore");
        }
    }
}
