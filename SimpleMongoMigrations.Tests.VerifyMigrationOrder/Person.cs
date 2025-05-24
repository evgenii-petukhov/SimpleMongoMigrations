using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SimpleMongoMigrations.Tests.VerifyMigrationOrder
{
    public class Person
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? Data { get; set; }
    }
}
