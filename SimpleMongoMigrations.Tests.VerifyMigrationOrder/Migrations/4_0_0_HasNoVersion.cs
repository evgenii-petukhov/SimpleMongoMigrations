using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Tests.VerifyMigrationOrder.Helpers;

namespace SimpleMongoMigrations.Tests.VerifyMigrationOrder.Migrations
{
    public class _4_0_0_HasNoVersion : IMigration
    {
        public Task UpAsync(IMongoDatabase database, CancellationToken cancellationToken)
        {
            return MongoHelper.AppendTextToAllPersonsDataAsync(database, "no version", cancellationToken);
        }
    }
}
