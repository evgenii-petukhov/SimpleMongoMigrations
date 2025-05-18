# SimpleMongoMigrations

**SimpleMongoMigrations** is a lightweight wrapper around the official [MongoDB C# Driver](https://github.com/mongodb/mongo-csharp-driver) designed to help you manage document migrations in your MongoDB database. It supports standalone MongoDB instances, Azure CosmosDB for MongoDB, and AWS DocumentDB.

This project is inspired by and based on [MongoDBMigrations](https://bitbucket.org/i_am_a_kernel/mongodbmigrations/).

## Why a new package?

I decided to create a new NuGet package instead of using the original MongoDBMigrations or its forks, such as [MongoDBMigrations (RZ version)](https://github.com/ruxo/MongoDbMigrations), for several reasons:

- **Dependency issues**: MongoDBMigrations relies on an outdated `MongoDB.Driver` version (2.14.1), which prevented me from upgrading my solutions.
- **Lack of support**: The original project appears to be unmaintained, with long delays in responding to issues and merging pull requests.
- **Learning opportunity**: I wanted to gain hands-on experience in managing NuGet packages and setting up GitHub pipelines.

## How to use

```csharp
new MigrationEngine(
    "mongodb://localhost:27017", // connection string
    "TestDB", // database name
    Assembly.GetAssembly(typeof(_001_AddDefaultData)) // assembly to scan for migrations
).Run();
```

## Simple migration example

See demos for [.NET 6](https://github.com/evgenii-petukhov/SimpleMongoMigrations/tree/master/SimpleMongoMigrations.Tests.ConsoleNet6) and [.NET Framework 4.7.2](https://github.com/evgenii-petukhov/SimpleMongoMigrations/tree/master/SimpleMongoMigrations.Tests.ConsoleNet472).

```csharp
namespace SimpleMongoMigrations.Tests.Migrations
{
    [Version("0.0.3")] // You can also use numbers for versions, such as 1, 2, 3, etc.
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
```

## License

SimpleMongoMigrations is licensed under the [MIT License](https://github.com/evgenii-petukhov/SimpleMongoMigrations/blob/master/LICENSE).
