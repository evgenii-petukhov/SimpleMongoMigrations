using MongoDB.Driver;

namespace SimpleMongoMigrations.Abstractions
{
    public interface ITransactionalMigration : IMigration
    {
        void Up(IMongoDatabase database, IClientSessionHandle session);
    }
}
