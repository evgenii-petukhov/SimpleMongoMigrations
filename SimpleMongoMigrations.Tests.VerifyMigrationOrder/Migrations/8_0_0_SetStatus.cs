using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using SimpleMongoMigrations.Tests.VerifyMigrationOrder.Helpers;

namespace SimpleMongoMigrations.Tests.VerifyMigrationOrder.Migrations
{
    [Version("8.0.0")]
    [Name("Sets status")]
    public class _8_0_0_SetStatus : IMigration
    {
        public Task UpAsync(IMongoDatabase database, CancellationToken cancellationToken)
        {
            return MongoHelper.AppendTextToAllPersonsDataAsync(database, "married", cancellationToken);
        }
    }
}
