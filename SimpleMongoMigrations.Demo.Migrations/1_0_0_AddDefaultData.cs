using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using SimpleMongoMigrations.Demo.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleMongoMigrations.Demo.Migrations
{
    [Version("1.0.0")]
    [Name("Populates the City collection with sample data")]
    public class _1_0_0_AddDefaultData : ITransactionalMigration
    {
        private readonly List<City> _cities = new List<City>
        {
            new City
            {
                Name = "London",
                Country = "England"
            },
            new City
            {
                Name = "Milan",
                Country = "Italy"
            },
            new City
            {
                Name = "Barcelona",
                Country = "Spain"
            },
            new City
            {
                Name = "Berlin",
                Country = "Germany"
            },
            new City
            {
                Name = "Paris",
                Country = "France"
            }
        };

        public Task UpAsync(
            IMongoDatabase database,
            CancellationToken cancellationToken)
        {
            return database.GetCollection<City>(nameof(City)).InsertManyAsync(
                _cities,
                cancellationToken: cancellationToken);
        }

        public Task UpAsync(
            IMongoDatabase database,
            IClientSessionHandle session,
            CancellationToken cancellationToken)
        {
            return database.GetCollection<City>(nameof(City)).InsertManyAsync(
                session,
                _cities,
                cancellationToken: cancellationToken);
        }
    }
}
