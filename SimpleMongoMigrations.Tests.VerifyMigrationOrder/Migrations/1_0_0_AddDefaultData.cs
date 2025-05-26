using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;

namespace SimpleMongoMigrations.Tests.VerifyMigrationOrder.Migrations
{
    [Version("1.0.0")]
    [Name("Adds default data")]
    public class _1_0_0_AddDefaultData : IMigration
    {
        public Task UpAsync(IMongoDatabase database, CancellationToken cancellationToken)
        {
            return database.GetCollection<Person>(nameof(Person)).InsertOneAsync(new Person
            {
                Data = "Olivia, "
            }, cancellationToken: cancellationToken);
        }
    }
}
