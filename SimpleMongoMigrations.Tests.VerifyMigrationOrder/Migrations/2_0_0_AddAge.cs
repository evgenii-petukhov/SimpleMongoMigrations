using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using SimpleMongoMigrations.Tests.VerifyMigrationOrder.Helpers;

namespace SimpleMongoMigrations.Tests.VerifyMigrationOrder.Migrations
{
    [Version("2.0.0")]
    [Name("Adds age")]
    public class _2_0_0_AddAge : IMigration
    {
        public Task UpAsync(IMongoDatabase database, CancellationToken cancellationToken)
        {
            return MongoHelper.AppendTextToAllPersonsDataAsync(database, "28 y.o., ", cancellationToken);
        }
    }
}
