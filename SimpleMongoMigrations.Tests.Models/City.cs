using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SimpleMongoMigrations.ConsoleAppNet6.Models
{
    public class City
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

        public string CountryCode { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }
    }
}
