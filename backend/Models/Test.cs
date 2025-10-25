using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConsistencyHub.Models
{
    public class Test
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("message")]
        public string Message { get; set; } = string.Empty;
    }
}
