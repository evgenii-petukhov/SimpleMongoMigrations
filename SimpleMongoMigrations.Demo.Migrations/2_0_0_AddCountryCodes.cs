using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using SimpleMongoMigrations.Demo.Models;

namespace SimpleMongoMigrations.Demo.Migrations
{
    [Version("2.0.0")]
    [Name("Adds a country code for each entry of the City collection")]
    public class _2_0_0_AddCountryCodes : ITransactionalMigration
    {
        public void Up(IMongoDatabase database)
        {
            database.GetCollection<City>(nameof(City)).UpdateOne(
                Builders<City>.Filter.Eq(x => x.Name, "London"),
                Builders<City>.Update.Set(x => x.CountryCode, "GB"));

            database.GetCollection<City>(nameof(City)).UpdateOne(
                Builders<City>.Filter.Eq(x => x.Name, "Milan"),
                Builders<City>.Update.Set(x => x.CountryCode, "IT"));

            database.GetCollection<City>(nameof(City)).UpdateOne(
                Builders<City>.Filter.Eq(x => x.Name, "Barcelona"),
                Builders<City>.Update.Set(x => x.CountryCode, "SP"));

            database.GetCollection<City>(nameof(City)).UpdateOne(
                Builders<City>.Filter.Eq(x => x.Name, "Berlin"),
                Builders<City>.Update.Set(x => x.CountryCode, "DE"));

            database.GetCollection<City>(nameof(City)).UpdateOne(
                Builders<City>.Filter.Eq(x => x.Name, "Paris"),
                Builders<City>.Update.Set(x => x.CountryCode, "FR"));
        }

        public void Up(IMongoDatabase database, IClientSessionHandle session)
        {
            database.GetCollection<City>(nameof(City)).UpdateOne(
                session,
                Builders<City>.Filter.Eq(x => x.Name, "London"),
                Builders<City>.Update.Set(x => x.CountryCode, "GB"));

            database.GetCollection<City>(nameof(City)).UpdateOne(
                session,
                Builders<City>.Filter.Eq(x => x.Name, "Milan"),
                Builders<City>.Update.Set(x => x.CountryCode, "IT"));

            database.GetCollection<City>(nameof(City)).UpdateOne(
                session,
                Builders<City>.Filter.Eq(x => x.Name, "Barcelona"),
                Builders<City>.Update.Set(x => x.CountryCode, "SP"));

            database.GetCollection<City>(nameof(City)).UpdateOne(
                session,
                Builders<City>.Filter.Eq(x => x.Name, "Berlin"),
                Builders<City>.Update.Set(x => x.CountryCode, "DE"));

            database.GetCollection<City>(nameof(City)).UpdateOne(
                session,
                Builders<City>.Filter.Eq(x => x.Name, "Paris"),
                Builders<City>.Update.Set(x => x.CountryCode, "FR"));
        }
    }
}
