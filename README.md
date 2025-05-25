# SimpleMongoMigrations

**SimpleMongoMigrations** is a lightweight wrapper around the official [MongoDB C# Driver](https://github.com/mongodb/mongo-csharp-driver) designed to help you manage document migrations in your MongoDB database. It supports standalone MongoDB instances, Azure CosmosDB for MongoDB, and AWS DocumentDB.

This project is inspired by and based on [MongoDBMigrations](https://bitbucket.org/i_am_a_kernel/mongodbmigrations/).

## Why a new package?

I decided to create a new NuGet package instead of using the original MongoDBMigrations or its forks, such as [MongoDBMigrations (RZ version)](https://github.com/ruxo/MongoDbMigrations), for several reasons:

- **Dependency issues**: MongoDBMigrations relies on an outdated `MongoDB.Driver` version (2.14.1), which prevented me from upgrading my solutions.
- **Lack of support**: The original project appears to be unmaintained, with long delays in responding to issues and merging pull requests.
- **Learning opportunity**: I wanted to gain hands-on experience in managing NuGet packages and setting up GitHub pipelines.

## How to use

The migration engine can be set up and configured with a fluent API:

```csharp
MigrationEngineBuilder
    .Create()
    .WithConnectionString("mongodb://localhost:27017")
    .WithDatabase("TestDB")
    .WithAssembly(Assembly.GetAssembly(typeof(_1_0_0_AddDefaultData)))
    .Build()
    .Run();
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
    [Version("4")]
    [Name("Adds a unique index by name")]
    public class _4_0_0_AddIndexByName : IMigration
    {
        public void Up(IMongoDatabase database, IClientSessionHandle session)
        {
            database.GetCollection<City>(nameof(City)).Indexes.CreateOne(
                // session, // Uncomment this line if you specified transactionScope in your settings and want to use transactions
                new CreateIndexModel<City>(
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

Transactions help ensure your database remains in a consistent state if the migration process fails.

To specify how migrations are wrapped in transactions, pass a `TransactionScope` value to `WithTransactionScope` when configuring the `MigrationEngine`. There are three options:

- **None** – Transactions are not used (default).
- **SingleMigration** – Each migration is wrapped in a separate transaction.
- **AllMigrations** – All migrations are wrapped in a single transaction.

```csharp
MigrationEngineBuilder
    .Create()
    .WithConnectionString("mongodb://localhost:27017")
    .WithDatabase("TestDB")
    .WithAssembly(Assembly.GetAssembly(typeof(_1_0_0_AddDefaultData)))
    .WithTransactionScope(TransactionScope.AllMigrations) // Optional, can be omitted if not needed
    .Build()
    .Run();
```

When using either of the last two options, an instance of `IClientSessionHandle` is passed to the `Up` method of your migrations. You should pass this session to all database operations, for example:

```csharp
public void Up(IMongoDatabase database, IClientSessionHandle session)
{
    database.GetCollection<City>(nameof(City)).UpdateOne(
        session,
        Builders<City>.Filter.Eq(x => x.Name, "London"),
        Builders<City>.Update.Set(x => x.CountryCode, "GB"));
}
```

> **Note:** Multi-document transactions are supported only on certain MongoDB deployments:
> - **Replica Set** (MongoDB 4.0+)
> - **Sharded Cluster** (MongoDB 4.2+)
> - **MongoDB Atlas** (M0/M2/M5 Free/Shared and M10+ Dedicated Clusters)
> - **Azure Cosmos DB** (MongoDB API v4.0+)
>
> Standalone MongoDB instances do **not** support multi-document transactions. The migration engine automatically detects whether transactions are supported. If transactions are not supported, the transaction scope specified in `WithTransactionScope` will be ignored, and the `session` parameter of the `Up` method will be `null`.
>
> **Limitations:**  
> - All operations in a transaction must be on collections within the same database.
> - Transactions have duration and size limits (e.g., 60 seconds and 16MB in MongoDB).
> - Cosmos DB supports multi-document transactions only within a single partition key value.
>
> Ensure your migration logic is compatible with these limitations and always test transactional migrations in your target environment.

## License

SimpleMongoMigrations is licensed under the [MIT License](https://github.com/evgenii-petukhov/SimpleMongoMigrations/blob/master/LICENSE).