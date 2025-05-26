using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleMongoMigrations.Abstractions
{
    public interface ITransactionalMigration : IMigration
    {
        Task UpAsync(IMongoDatabase database, IClientSessionHandle session, CancellationToken cancellationToken);
    }
}
