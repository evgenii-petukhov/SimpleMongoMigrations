using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using SimpleMongoMigrations.Tests.VerifyMigrationOrder.Helpers;

namespace SimpleMongoMigrations.Tests.VerifyMigrationOrder.Migrations
{
    [Version("3")]
    public class _3_0_0_AddCity : IMigration
    {
        public Task UpAsync(IMongoDatabase database, CancellationToken cancellationToken)
        {
            return MongoHelper.AppendTextToAllPersonsDataAsync(database, "from Paris, ", cancellationToken);
        }
    }
}
