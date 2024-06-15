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
        Role = mongoDatabase.GetCollection<TRole>(nameof(Role));
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

    public async Task<IResult> UpdateAsync(TRole role, CancellationToken cancellationToken = default)
    {

        try
        {
            var existRole = await Role.Find(x => x.Id == role.Id).SingleOrDefaultAsync(cancellationToken);

            if (existRole is null)
                return Result<TRole>.Failure(message: "Role not found");

            existRole.RoleName = role.RoleName ?? existRole.RoleName;
            existRole.Description = role.Description ?? existRole.Description;
            await Role.ReplaceOneAsync(x => x.Id == role.Id, existRole, cancellationToken: cancellationToken);
            return Result<TRole>.Success(existRole, message: "Role updated successfully");
        }
        catch
        {
            return Result<TRole>.Failure(message: "Id is not valid!");

        }


    }

    public async Task<IResult> DeleteAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var existRole = await Role.Find(x => x.RoleName == roleName).SingleOrDefaultAsync(cancellationToken);
        if (existRole is null)
            return Result<TRole>.Failure(message: "Role not found!");

        await Role.DeleteOneAsync(x => x.RoleName == roleName, cancellationToken: cancellationToken);
        return Result<TRole>.Success(existRole, message: "Role deleted successfully");
    }

    public async Task<IResult<IEnumerable<TRole>>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await Role.Find(_ => true).ToListAsync(cancellationToken);
        return Result<IEnumerable<TRole>>.Success(roles);
    }

    public async Task<IResult<TRole>> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var role = await Role.Find(x => x.RoleName == roleName).SingleOrDefaultAsync(cancellationToken);
        if (role is null)
            return Result<TRole>.Failure(message: "Role not found!");

        return Result<TRole>.Success(role);
    }

    public async Task<IResult<TRole>> GetRoleByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var role = await Role.Find(x => x.Id == id).SingleOrDefaultAsync(cancellationToken);
            if (id.Length != 24)
                return Result<TRole>.Failure(message: "Id is not valid!");

            if (role is null)
                return Result<TRole>.Failure(message: "Role not found!");

            return Result<TRole>.Success(role);

        }
        catch
        {
            return Result<TRole>.Failure(message: "Id is not valid!");
        }

    }



}
