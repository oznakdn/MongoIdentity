using MongoDB.Bson.Serialization.Attributes;

namespace Gleeman.AspNetCore.MongoIdentity.Models;

public abstract class MongoIdentityUser
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string EmailAddress { get; set; }
    public string HashedPassword { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;

}
