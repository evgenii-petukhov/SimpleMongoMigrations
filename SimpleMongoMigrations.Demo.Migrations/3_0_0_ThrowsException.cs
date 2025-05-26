using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleMongoMigrations.Demo.Migrations
{
    [Version("3.0.0")]
    [Name("Adds a unique index by name")]
    [Ignore]
    public class _3_0_0_ThrowsException : IMigration
    {
        public Task UpAsync(IMongoDatabase database, CancellationToken cancellationToken)
        {
            throw new System.Exception("This migration is not implemented yet. Please implement it before running the migrations.");
        }
    }
}
