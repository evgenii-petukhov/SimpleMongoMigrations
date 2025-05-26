using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;

namespace SimpleMongoMigrations.Tests.VerifyMigrationOrder.Migrations
{
    [Version(7)]
    [Name("Adds another person")]
    public class _7_0_0_AddAnotherPerson : IMigration
    {
        public Task UpAsync(IMongoDatabase database, CancellationToken cancellationToken)
        {
            return database.GetCollection<Person>(nameof(Person)).InsertOneAsync(new Person
            {
                Data = "Lucas, "
            }, cancellationToken: cancellationToken);
        }
    }
}
