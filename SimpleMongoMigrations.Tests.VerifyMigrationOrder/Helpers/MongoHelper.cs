using MongoDB.Driver;

namespace SimpleMongoMigrations.Tests.VerifyMigrationOrder.Helpers
{
    public static class MongoHelper
    {
        public static void AppendTextToAllPersonsData(IMongoDatabase database, string textToAppend)
        {
            var collection = database.GetCollection<Person>(nameof(Person));

            var people = collection.Find(Builders<Person>.Filter.Empty).ToList();

            var writes = people.Select(person =>
            {
                person.Data += textToAppend;
                return new ReplaceOneModel<Person>(
                    Builders<Person>.Filter.Eq(p => p.Id, person.Id),
                    person
                );
            }).ToList();

            collection.BulkWrite(writes);
        }
    }
}
