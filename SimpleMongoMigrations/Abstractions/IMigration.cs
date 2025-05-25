using MongoDB.Driver;

namespace SimpleMongoMigrations.Abstractions
{
    public interface IMigration
    {
        void Up(IMongoDatabase database, IClientSessionHandle session);
    }
}
