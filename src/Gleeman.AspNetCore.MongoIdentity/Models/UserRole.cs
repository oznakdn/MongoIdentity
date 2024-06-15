using MongoDB.Bson.Serialization.Attributes;

namespace Gleeman.AspNetCore.MongoIdentity.Models;

public sealed class UserRole
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }
    public string UserId { get; set; }
    public string RoleId { get; set; }
}
