using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace SimpleMongoMigrations.Models
{
    internal class Migration
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("n")]
        public string Name { get; set; }

        [BsonElement("v")]
        public Version Version { get; set; }

        [BsonElement("d")]
        public bool IsUp { get; set; }

        [BsonElement("applied")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc, Representation = BsonType.DateTime)]
        public DateTime TimeStamp { get; set; }
    }
}
