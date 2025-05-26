using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;

namespace SimpleMongoMigrations.Tests.VerifyMigrationOrder.Migrations
{
    [Version(7)]
    [Name("Adds another person")]
    public class _7_0_0_AddAnotherPerson : IMigration
    {
        public void Up(IMongoDatabase database)
        {
            database.GetCollection<Person>(nameof(Person)).InsertOne(new Person
            {
                Data = "Lucas, "
            });
        }
    }
}
