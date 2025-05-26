using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using SimpleMongoMigrations.Demo.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleMongoMigrations.Demo.Migrations
{
    [Version("4.0.0")]
    [Name("Adds a country code for each entry of the City collection")]
    public class _4_0_0_AddCountryCodes : ITransactionalMigration
    {
        public async Task UpAsync(
            IMongoDatabase database,
            CancellationToken cancellationToken)
        {
            await database.GetCollection<City>(nameof(City)).UpdateOneAsync(
                Builders<City>.Filter.Eq(x => x.Name, "London"),
                Builders<City>.Update.Set(x => x.CountryCode, "GB"),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await database.GetCollection<City>(nameof(City)).UpdateOneAsync(
                Builders<City>.Filter.Eq(x => x.Name, "Milan"),
                Builders<City>.Update.Set(x => x.CountryCode, "IT"),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await database.GetCollection<City>(nameof(City)).UpdateOneAsync(
                Builders<City>.Filter.Eq(x => x.Name, "Barcelona"),
                Builders<City>.Update.Set(x => x.CountryCode, "SP"),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await database.GetCollection<City>(nameof(City)).UpdateOneAsync(
                Builders<City>.Filter.Eq(x => x.Name, "Berlin"),
                Builders<City>.Update.Set(x => x.CountryCode, "DE"),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await database.GetCollection<City>(nameof(City)).UpdateOneAsync(
                Builders<City>.Filter.Eq(x => x.Name, "Paris"),
                Builders<City>.Update.Set(x => x.CountryCode, "FR"),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task UpAsync(
            IMongoDatabase database,
            IClientSessionHandle session,
            CancellationToken cancellationToken)
        {
            await database.GetCollection<City>(nameof(City)).UpdateOneAsync(
                session,
                Builders<City>.Filter.Eq(x => x.Name, "London"),
                Builders<City>.Update.Set(x => x.CountryCode, "GB"),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await database.GetCollection<City>(nameof(City)).UpdateOneAsync(
                session,
                Builders<City>.Filter.Eq(x => x.Name, "Milan"),
                Builders<City>.Update.Set(x => x.CountryCode, "IT"),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await database.GetCollection<City>(nameof(City)).UpdateOneAsync(
                session,
                Builders<City>.Filter.Eq(x => x.Name, "Barcelona"),
                Builders<City>.Update.Set(x => x.CountryCode, "SP"),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await database.GetCollection<City>(nameof(City)).UpdateOneAsync(
                session,
                Builders<City>.Filter.Eq(x => x.Name, "Berlin"),
                Builders<City>.Update.Set(x => x.CountryCode, "DE"),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await database.GetCollection<City>(nameof(City)).UpdateOneAsync(
                session,
                Builders<City>.Filter.Eq(x => x.Name, "Paris"),
                Builders<City>.Update.Set(x => x.CountryCode, "FR"),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
