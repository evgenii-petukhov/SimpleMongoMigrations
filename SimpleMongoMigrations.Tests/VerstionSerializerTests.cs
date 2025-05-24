using FluentAssertions;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;

namespace SimpleMongoMigrations.Tests
{
    [TestFixture]
    public class VerstionSerializerTests
    {
        private VerstionSerializer _serializer;

        [SetUp]
        public void SetUp()
        {
            _serializer = new VerstionSerializer();
        }

        [Test]
        public void Serialize_ShouldWriteVersionAsString()
        {
            var version = new Version(1, 2, 3);
            var writer = new BsonDocumentWriter([]);
            writer.WriteStartDocument();
            writer.WriteName("version");

            _serializer.Serialize(BsonSerializationContext.CreateRoot(writer), default, version);

            writer.WriteEndDocument();
            var doc = writer.Document;
            doc["version"].Should().Be("1.2.3");
        }

        [Test]
        public void Deserialize_ShouldReadVersionFromString()
        {
            var doc = new BsonDocument { { "version", "4.5.6" } };
            var reader = new BsonDocumentReader(doc);
            reader.ReadStartDocument();
            reader.ReadName("version");

            var version = _serializer.Deserialize(BsonDeserializationContext.CreateRoot(reader), default);

            version.Major.Should().Be(4);
            version.Minor.Should().Be(5);
            version.Revision.Should().Be(6);
            reader.ReadEndDocument();
        }

        [Test]
        public void Serialize_And_Deserialize_ShouldRoundTrip()
        {
            var original = new Version(7, 8, 9);
            var doc = new BsonDocument();
            var writer = new BsonDocumentWriter(doc);
            writer.WriteStartDocument();
            writer.WriteName("version");
            _serializer.Serialize(BsonSerializationContext.CreateRoot(writer), default, original);
            writer.WriteEndDocument();

            var reader = new BsonDocumentReader(doc);
            reader.ReadStartDocument();
            reader.ReadName("version");
            var deserialized = _serializer.Deserialize(BsonDeserializationContext.CreateRoot(reader), default);
            reader.ReadEndDocument();

            deserialized.Should().Be(original);
        }

        [Test]
        public void Deserialize_WithInvalidVersionString_ShouldThrow()
        {
            var doc = new BsonDocument { { "version", "invalid" } };
            var reader = new BsonDocumentReader(doc);
            reader.ReadStartDocument();
            reader.ReadName("version");

            Action act = () => _serializer.Deserialize(BsonDeserializationContext.CreateRoot(reader), default);

            act.Should().Throw<Exceptions.InvalidVersionException>();
            reader.ReadEndDocument();
        }

        [Test]
        public void Serialize_WithNullContext_ShouldThrow()
        {
            var version = new Version(1, 2, 3);
            Action act = () => _serializer.Serialize(null, default, version);
            act.Should().Throw<NullReferenceException>();
        }

        [Test]
        public void Deserialize_WithNullContext_ShouldThrow()
        {
            Action act = () => _serializer.Deserialize(null, default);
            act.Should().Throw<NullReferenceException>();
        }
    }
}
