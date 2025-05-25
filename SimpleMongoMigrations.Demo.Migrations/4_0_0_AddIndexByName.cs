using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using SimpleMongoMigrations.Demo.Models;

namespace SimpleMongoMigrations.Demo.Migrations
{
    [Version("4")]
    [Name("Adds a unique index by name")]
    public class _4_0_0_AddIndexByName : IMigration
    {
        public void Up(IMongoDatabase database, IClientSessionHandle session)
        {
            database.GetCollection<City>(nameof(City)).Indexes.CreateOne(
                //session, // Uncomment this line if you want to use a session for the insert operation
                new CreateIndexModel<City>(
                    Builders<City>.IndexKeys.Ascending(x => x.Name),
                    new CreateIndexOptions
                    {
                        Unique = true
                    }));
        }
    }
}
