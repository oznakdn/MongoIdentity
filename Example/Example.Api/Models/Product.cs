using MongoDB.Bson.Serialization.Attributes;

namespace Example.Api.Models;

public class Product
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }
    public string Name { get; set; }
}
