using Gleeman.AspNetCore.MongoIdentity.Models;
using Gleeman.AspNetCore.MongoIdentity.Options;
using MongoDB.Driver;

namespace Gleeman.AspNetCore.MongoIdentity.Managers;

public sealed class MongoRoleManager<TRole>
    where TRole : MongoIdentityRole
{
    private IMongoCollection<TRole> Role { get; }

    public MongoRoleManager(MongoOption option)
    {
        var mongoClient = new MongoClient(option.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(option.DatabaseName);
        Role = mongoDatabase.GetCollection<TRole>("Roles");
    }


}
