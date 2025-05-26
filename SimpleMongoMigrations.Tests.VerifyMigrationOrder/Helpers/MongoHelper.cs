using MongoDB.Driver;

namespace SimpleMongoMigrations.Tests.VerifyMigrationOrder.Helpers
{
    public static class MongoHelper
    {
        public static async Task AppendTextToAllPersonsDataAsync(
            IMongoDatabase database,
            string textToAppend,
            CancellationToken cancellationToken)
        {
            var collection = database.GetCollection<Person>(nameof(Person));

            var people = await collection.Find(Builders<Person>.Filter.Empty)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var writes = people.Select(person =>
            {
                person.Data += textToAppend;
                return new ReplaceOneModel<Person>(
                    Builders<Person>.Filter.Eq(p => p.Id, person.Id),
                    person
                );
            }).ToList();

            await collection.BulkWriteAsync(writes, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
