# SimpleMongoMigrations

**SimpleMongoMigrations** is a lightweight wrapper around the official [MongoDB C# Driver](https://github.com/mongodb/mongo-csharp-driver) designed to help you manage document migrations in your MongoDB database. It supports standalone MongoDB instances, Azure CosmosDB for MongoDB, and AWS DocumentDB.

This project is inspired by and based on [MongoDBMigrations](https://bitbucket.org/i_am_a_kernel/mongodbmigrations/).

### Why a new package?

There are several reasons why I decided to create a new NuGet package instead of using the original MongoDBMigrations or its forks, such as [MongoDBMigrations (RZ version)](https://github.com/ruxo/MongoDbMigrations):

- **Dependency issues**: MongoDBMigrations relies on an outdated `MongoDB.Driver` version (2.14.1), which prevented me from upgrading my solutions.
- **Lack of support**: The original project appears to be unmaintained, with long delays in responding to issues and merging pull requests.
- **Learning opportunity**: I wanted to gain hands-on experience in managing NuGet packages and setting up GitHub pipelines.

## License

SimpleMongoMigrations is licensed under the [MIT License](https://github.com/evgenii-petukhov/SimpleMongoMigrations/blob/master/LICENSE).