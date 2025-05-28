using MongoDB.Driver;
using SimpleMongoMigrations.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleMongoMigrations.Abstractions
{
    internal interface IMigrationRepository
    {
        Task<Migration> GetMostRecentAppliedMigrationAsync(CancellationToken cancellationToken);

        Task SaveMigrationAsync(IClientSessionHandle session, Version version, string name, CancellationToken cancellationToken);

        Task SaveMigrationAsync(Version version, string name, CancellationToken cancellationToken);
    }
}
