using MongoDB.Bson.Serialization.Attributes;

namespace Gleeman.AspNetCore.MongoIdentity.Models;

public abstract class MongoIdentityRole
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }
    public string RoleName { get; set; }
    public string Description { get; set; } = string.Empty;

}
