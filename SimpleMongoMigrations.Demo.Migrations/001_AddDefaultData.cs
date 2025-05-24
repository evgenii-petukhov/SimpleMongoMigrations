using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using SimpleMongoMigrations.Demo.Models;
using System.Collections.Generic;

namespace SimpleMongoMigrations.Demo.Migrations
{
    [Version("0.0.1")]
    [Name("Populates the City collection with sample data")]
    public class _001_AddDefaultData : IMigration
    {
        public void Up(IMongoDatabase database)
        {
            database.GetCollection<City>(nameof(City)).InsertMany(
                new List<City>
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
                });
        }
    }
}
