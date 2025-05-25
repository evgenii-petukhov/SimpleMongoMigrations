using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;

namespace SimpleMongoMigrations.Tests.VerifyMigrationOrder.Migrations
{
    [Version("1.0.0")]
    [Name("Adds default data")]
    public class _001_AddDefaultData : IMigration
    {
        public void Up(IMongoDatabase database)
        {
            database.GetCollection<Person>(nameof(Person)).InsertOne(new Person
            {
                Data = "Olivia, "
            });
        }
    }
}
