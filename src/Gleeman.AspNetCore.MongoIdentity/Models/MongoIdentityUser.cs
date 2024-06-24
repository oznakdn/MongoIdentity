using MongoDB.Bson.Serialization.Attributes;

namespace Gleeman.AspNetCore.MongoIdentity.Models;

public class MongoIdentityUser
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }
    public string? UserName { get; set; }
    public string EmailAddress { get; set; }
    public string HashedPassword { get; set; }
    public string? PhoneNumber { get; set; }

}
