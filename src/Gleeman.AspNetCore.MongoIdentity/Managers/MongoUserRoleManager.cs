using Gleeman.AspNetCore.MongoIdentity.Models;
using Gleeman.AspNetCore.MongoIdentity.Options;
using Gleeman.AspNetCore.MongoIdentity.Results;
using MongoDB.Driver;

namespace Gleeman.AspNetCore.MongoIdentity.Managers;

public class MongoUserRoleManager<TUser, TRole>
    where TUser : MongoIdentityUser
    where TRole : MongoIdentityRole
{
    private IMongoCollection<UserRole> UserRole { get; }
    private IMongoCollection<TUser> User { get; }
    private IMongoCollection<TRole> Role { get; }

    public MongoUserRoleManager(MongoOption option)
    {
        var mongoClient = new MongoClient(option.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(option.DatabaseName);
        UserRole = mongoDatabase.GetCollection<UserRole>(nameof(UserRole));
        User = mongoDatabase.GetCollection<TUser>(nameof(User));
        Role = mongoDatabase.GetCollection<TRole>(nameof(Role));
    }

    public async Task<IResult> AddRoleToUserAsync(TUser user, string roleName, CancellationToken cancellationToken = default)
    {


        if (string.IsNullOrWhiteSpace(roleName))
        {
            return Result.Failure("Role name is required!");
        }


        var existUser = await User.Find(x => x.Id == user.Id).SingleOrDefaultAsync(cancellationToken);
        if (existUser is null)
            return Result.Failure("User not found!");

        var existRole = await Role.Find(x => x.RoleName == roleName).SingleOrDefaultAsync(cancellationToken);
        if (existRole is null)
            return Result.Failure("Role not found!");


        var userRole = await UserRole
       .Find(x => x.UserId == existUser.Id && x.RoleId == existRole.Id)
       .SingleOrDefaultAsync(cancellationToken);

        if (userRole is not null)
            if (userRole is not null)
                return Result.Failure("User already has this role!");


        await UserRole
            .InsertOneAsync(new UserRole
            {
                UserId = existUser.Id,
                RoleId = existRole.Id
            }, cancellationToken: cancellationToken);

        return Result.Success("Role assigned successfully!");
    }

    public async Task<IResult> RemoveRoleFromUserAsync(TUser user, string roleName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return Result.Failure("Role name is required!");
        }


        var existUser = await User.Find(x => x.Id == user.Id).SingleOrDefaultAsync(cancellationToken);
        if (existUser is null)
            return Result.Failure("User not found!");

        var existRole = await Role.Find(x => x.RoleName == roleName).SingleOrDefaultAsync(cancellationToken);
        if (existRole is null)
            return Result.Failure("Role not found!");


        var userRole = await UserRole
       .Find(x => x.UserId == existUser.Id && x.RoleId == existRole.Id)
       .SingleOrDefaultAsync(cancellationToken);

        if (userRole is null)
            if (userRole is not null)
                return Result.Failure("User has no this role!");


        await UserRole
            .FindOneAndDeleteAsync(x => x.UserId == existUser.Id && x.RoleId == existRole.Id, cancellationToken: cancellationToken);

        return Result.Success("Role deleted from the user successfully!");
    }

    public async Task<IResult<IEnumerable<TUser>>>GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {

     
        if (string.IsNullOrWhiteSpace(roleName))
            return Result<IEnumerable<TUser>>.Failure(message: "Role name is required!");

        var existRole = await Role.Find(x => x.RoleName == roleName).SingleOrDefaultAsync(cancellationToken);

        if (existRole is null)
            return Result<IEnumerable<TUser>>.Failure(message: "Role not found!");


        var userRoles = await UserRole
            .Find(x => x.RoleId == existRole.Id)
            .ToListAsync(cancellationToken);


        var users = new List<TUser>();

        userRoles.ForEach(role =>
        {
            var user = User
                .Find(x => x.Id == role.UserId)
                .SingleOrDefault();

            if(user is not null)
            {
                users.Add(user);
            }
        });


        return Result<IEnumerable<TUser>>.Success(users);
    }

    public async Task<IResult<bool>>GetUserInRoleAsync(string userId, string roleName , CancellationToken cancellationToken = default)
    {
        var erros = new List<string>();

        if (string.IsNullOrWhiteSpace(roleName))
            erros.Add("Role name is required!");

        if (string.IsNullOrWhiteSpace(userId))
            erros.Add("User Id is required!");

        if(erros.Count>0)
            return Result<bool>.Failure(errors: erros);


        var existRole = await Role.Find(x => x.RoleName == roleName).SingleOrDefaultAsync(cancellationToken);


        if (existRole is null)
            return Result<bool>.Failure(message: "Role not found!");


        var existUser = await User
            .Find(x => x.Id == userId)
            .SingleOrDefaultAsync(cancellationToken);


        if (existUser is null)
            return Result<bool>.Failure(message: "User not found!");


        var userRole = await UserRole
            .Find(x => x.RoleId == existRole.Id && x.UserId == existUser.Id)
            .SingleOrDefaultAsync(cancellationToken);


        if (userRole is null)
            return Result<bool>.Failure(message: "User has no this role!");


        return Result<bool>.Success(true);

    }


}



