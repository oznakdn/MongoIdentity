using MongoDB.Bson.Serialization.Attributes;

namespace Gleeman.AspNetCore.MongoIdentity.Models;

public sealed class UserToken
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Access { get; set; }
    public DateTime AccessExp { get; set; }
    public string Refresh { get; set; } = string.Empty;
    public DateTime? RefreshExp { get; set; }

}
