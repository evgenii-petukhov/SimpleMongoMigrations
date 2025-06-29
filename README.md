# SimpleMongoMigrations

**SimpleMongoMigrations** is a lightweight wrapper around the official [MongoDB C# Driver](https://github.com/mongodb/mongo-csharp-driver) designed to help you manage document migrations in your MongoDB database. It supports standalone MongoDB instances, Azure CosmosDB for MongoDB, and AWS DocumentDB.

This project is inspired by and based on [MongoDBMigrations](https://bitbucket.org/i_am_a_kernel/mongodbmigrations/).

## Why a new package?

I decided to create a new NuGet package instead of using the original MongoDBMigrations or its forks, such as [MongoDBMigrations (RZ version)](https://github.com/ruxo/MongoDbMigrations), for several reasons:

- **Dependency issues:** MongoDBMigrations relies on an outdated `MongoDB.Driver` version (2.14.1), which prevented me from upgrading my solutions.
- **Lack of support:** The original project appears to be unmaintained, with long delays in responding to issues and merging pull requests.
- **Learning opportunity:** I wanted to gain hands-on experience in managing NuGet packages and setting up GitHub pipelines.
- **Missing features:** The initial library does not support executing migrations asynchronously.

## How to use

See demos for [.NET 6](https://github.com/evgenii-petukhov/SimpleMongoMigrations/tree/master/SimpleMongoMigrations.Demo.ConsoleNet6) and [.NET Framework 4.7.2](https://github.com/evgenii-petukhov/SimpleMongoMigrations/tree/master/SimpleMongoMigrations.Demo.ConsoleNet472).

You can also see a [real-life usage example](https://github.com/evgenii-petukhov/LetsTalk.Server/blob/master/LetsTalk.Server.Persistence.MongoDB.Services/MongoDBServicesRegistration.cs) in another project.

The migration engine can be set up and configured with a fluent API:

```csharp
await MigrationEngineBuilder
    .Create()
    .WithConnectionString("mongodb://localhost:27017") // connection string
    .WithDatabase("TestDB") // database name
    .WithAssembly(Assembly.GetAssembly(typeof(_1_0_0_AddDefaultData))) // assembly to scan for migrations
    .Build()
    .RunAsync();
```

Unlike [MongoDBMigrations](https://bitbucket.org/i_am_a_kernel/mongodbmigrations/), the `Run` method does not expect a version. All migrations will be executed unless they are marked with the `Ignore` attribute.

### Simple migration example

You can also find migration samples in the [demo](https://github.com/evgenii-petukhov/SimpleMongoMigrations/tree/master/SimpleMongoMigrations.Demo.Migrations) and [test](https://github.com/evgenii-petukhov/SimpleMongoMigrations/tree/master/SimpleMongoMigrations.Tests.VerifyMigrationOrder/Migrations) projects.

The example below creates a new index on the `Name` field in the `City` collection. Each migration implements either the `IMigration` or `ITransactionalMigration` interface. Use `ITransactionalMigration` only if your environment supports transactions. Standalone MongoDB instances do not support transactions. The only difference between these interfaces is that `ITransactionalMigration` provides an `UpAsync` method with a session parameter, which should be passed to every database operation to enable rollback in case of failure. If the migration engine detects that your environment doesn't support transactions, the `UpAsync` method with a single parameter will be called as a fallback.

```csharp
using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using SimpleMongoMigrations.Demo.Models;

namespace SimpleMongoMigrations.Demo.Migrations
{
    [Version("1")]
    [Name("Adds a unique index by name")]
    public class _1_0_0_AddIndexByName : IMigration
    {
        public Task UpAsync(
            IMongoDatabase database,
            CancellationToken cancellationToken)
        {
            return database.GetCollection<City>(nameof(City)).Indexes.CreateOneAsync(
                new CreateIndexModel<City>(
                    Builders<City>.IndexKeys.Ascending(x => x.Name),
                    new CreateIndexOptions
                    {
                        Unique = true
                    }),
                cancellationToken: cancellationToken);
        }
    }
}
```

> **Note:** The `IMigration` interface does not include a `Down` method. This means you cannot roll back migrations or downgrade your database.

### Transactions

Transactions help ensure your database remains in a consistent state if the migration process fails.

To specify how migrations are wrapped in transactions, pass a `TransactionScope` value to `WithTransactionScope` when configuring the `MigrationEngine`. There are three options:

- **NoTransaction** – Transactions are not used (default).
- **PerMigration** – Each migration is wrapped in a separate transaction.
- **SingleTransaction** – All migrations are wrapped in a single transaction.

```csharp
await MigrationEngineBuilder
    .Create()
    .WithConnectionString("mongodb://localhost:27017") // connection string
    .WithDatabase("TestDB") // database name
    .WithAssembly(Assembly.GetAssembly(typeof(_1_0_0_AddIndexByName))) // assembly to scan for migrations
    .WithTransactionScope(TransactionScope.SingleTransaction) // Optional, can be omitted if not needed
    .Build()
    .RunAsync(default);
```

Use `ITransactionalMigration` only if your environment supports transactions in combination with the `SingleMigration` or `AllMigrations` options. An instance of `IClientSessionHandle` is passed to the `UpAsync` method of your migrations. You should pass this session to all database operations. If the migration engine detects that your environment doesn't support transactions, the `UpAsync` method with a single parameter will be called as a fallback.

```csharp
[Version("1")]
[Name("Adds a unique index by name")]
public class _1_0_0_AddIndexByName : ITransactionalMigration
{
    public Task UpAsync(
        IMongoDatabase database,
        CancellationToken cancellationToken)
    {
        return database.GetCollection<City>(nameof(City)).Indexes.CreateOneAsync(
            new CreateIndexModel<City>(
                Builders<City>.IndexKeys.Ascending(x => x.Name),
                new CreateIndexOptions
                {
                    Unique = true
                }),
            cancellationToken: cancellationToken);
    }

    public Task UpAsync(
        IMongoDatabase database,
        IClientSessionHandle session,
        CancellationToken cancellationToken)
    {
        return database.GetCollection<City>(nameof(City)).Indexes.CreateOneAsync(
            session,
            new CreateIndexModel<City>(
                Builders<City>.IndexKeys.Ascending(x => x.Name),
                new CreateIndexOptions
                {
                    Unique = true
                }),
            cancellationToken: cancellationToken);
    }
}
```

**Note:** Multi-document transactions are supported only on certain MongoDB deployments:

| MongoDB Deployment Type                | Supports Transactions | Notes                                                            |
| -------------------------------------- | :-------------------: | ---------------------------------------------------------------- |
| Standalone MongoDB                     |         ❌ No         | Single-node deployments do not support transactions.             |
| Replica Set (MongoDB 4.0+)             |        ✅ Yes         | Multi-document transactions supported starting with MongoDB 4.0. |
| Sharded Cluster (MongoDB 4.2+)         |        ✅ Yes         | Requires MongoDB 4.2+ and sharded collections.                   |
| MongoDB Atlas Free/Shared (M0/M2/M5)   |        ✅ Yes         | Uses replica sets by default. Transactions are available.        |
| MongoDB Atlas Dedicated Cluster (M10+) |        ✅ Yes         | Fully supports transactions.                                     |
| Azure Cosmos DB (MongoDB API v4.0+)    |   ✅ Yes (limited)    | Only within the same partition key; no cross-partition support.  |

Standalone MongoDB instances do **not** support multi-document transactions. The migration engine automatically detects whether transactions are supported. If transactions are not supported, the transaction scope specified in `WithTransactionScope` will be ignored, and the `session` parameter of the `UpAsync` method will be `null`.

**Limitations:**

- All operations in a transaction must be on collections within the same database.
- Transactions have duration and size limits (e.g., 60 seconds and 16MB in MongoDB).
- Cosmos DB supports multi-document transactions only within a single partition key value.

Ensure your migration logic is compatible with these limitations and always test transactional migrations in your target environment.

## Migration history compatibility

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

## License

SimpleMongoMigrations is licensed under the [MIT License](https://github.com/evgenii-petukhov/SimpleMongoMigrations/blob/master/LICENSE).
