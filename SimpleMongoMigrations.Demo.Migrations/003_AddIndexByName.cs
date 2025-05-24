using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using SimpleMongoMigrations.Demo.Models;

namespace SimpleMongoMigrations.Demo.Migrations
{
    [Version("0.0.3")]
    [Name("Adds a unique index by name")]
    public class _003_AddIndexByName : IMigration
    {
        public void Up(IMongoDatabase database)
        {
            database.GetCollection<City>(nameof(City)).Indexes.CreateOne(new CreateIndexModel<City>(
                Builders<City>.IndexKeys.Ascending(x => x.Name),
                new CreateIndexOptions
                {
                    Unique = true
                }));
        }
    }
}
