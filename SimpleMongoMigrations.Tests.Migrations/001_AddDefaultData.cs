using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using SimpleMongoMigrations.ConsoleAppNet6.Models;
using System.Collections.Generic;

namespace SimpleMongoMigrations.Tests.Migrations
{
    [Version("0.0.1")]
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
