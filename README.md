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

Unlike [MongoDBMigrations](https://bitbucket.org/i_am_a_kernel/mongodbmigrations/), the `Run` method does not expect a version. All migrations will be executed unless they are marked with the `Ignore` attribute.

## Simple migration example

See demos for [.NET 6](https://github.com/evgenii-petukhov/SimpleMongoMigrations/tree/master/SimpleMongoMigrations.Demo.ConsoleNet6) and [.NET Framework 4.7.2](https://github.com/evgenii-petukhov/SimpleMongoMigrations/tree/master/SimpleMongoMigrations.Demo.ConsoleNet472).

You can also find migration samples in the [demo](https://github.com/evgenii-petukhov/SimpleMongoMigrations/tree/master/SimpleMongoMigrations.Demo.Migrations) and [test](https://github.com/evgenii-petukhov/SimpleMongoMigrations/tree/master/SimpleMongoMigrations.Tests.VerifyMigrationOrder/Migrations) projects.

```csharp
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
```

> **Note:** The `IMigration` interface does not include a `Down` method. This means you cannot roll back migrations or downgrade your database.

## Migration history

SimpleMongoMigrations maintains migration history in a way that is compatible with [MongoDBMigrations](https://bitbucket.org/i_am_a_kernel/mongodbmigrations/). If you are already using MongoDBMigrations, you do not need to make any changes to your database—only minor code adjustments may be required to refactor your migrations.

Below is a sample entry created in the `_migrations` collection:

```
{
    "_id" : ObjectId("6828faf9642d86f79f782e4f"),
    "n" : "Adds a unique index by name",
    "v" : "0.0.3",
    "d" : true,
    "applied" : ISODate("2025-05-17T21:09:13.355+0000")
}
```

## Transactions

Transactions are not supported at the moment. However, support for transactions is planned for a future release.

## License

SimpleMongoMigrations is licensed under the [MIT License](https://github.com/evgenii-petukhov/SimpleMongoMigrations/blob/master/LICENSE).