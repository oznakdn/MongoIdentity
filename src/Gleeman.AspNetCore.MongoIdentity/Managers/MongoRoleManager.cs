using Gleeman.AspNetCore.MongoIdentity.Models;
using Gleeman.AspNetCore.MongoIdentity.Options;
using Gleeman.AspNetCore.MongoIdentity.Results;
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

    public async Task<IResult> CreateAsync(TRole role, CancellationToken cancellationToken = default)
    {
        var existRole = await Role.Find(x => x.RoleName == role.RoleName).SingleOrDefaultAsync(cancellationToken);

        if (existRole is not null)
        {
            return Result<TRole>.Failure(message: "Role already exists!");
        }

        await Role.InsertOneAsync(role, cancellationToken: cancellationToken);
        return Result<TRole>.Success(role);
    }


}
