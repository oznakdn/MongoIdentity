using Gleeman.AspNetCore.MongoIdentity.Helpers;
using Gleeman.AspNetCore.MongoIdentity.Models;
using Gleeman.AspNetCore.MongoIdentity.Options;
using Gleeman.AspNetCore.MongoIdentity.Results;
using MongoDB.Driver;

namespace Gleeman.AspNetCore.MongoIdentity.Managers;

public sealed class MongoUserManager<TUser>
    where TUser : MongoIdentityUser
{
    private IMongoCollection<TUser> MongoUser { get; }
    private IMongoCollection<UserToken> UserToken { get; }


    private readonly JwtHelper<TUser> _jwtHelper;

    public MongoUserManager(MongoOption option, JwtOption? jwtOption = null)
    {
        var mongoClient = new MongoClient(option.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(option.DatabaseName);
        MongoUser = mongoDatabase.GetCollection<TUser>("Users");
        UserToken = mongoDatabase.GetCollection<UserToken>("UserTokens");
        if (jwtOption is not null)
            _jwtHelper = new JwtHelper<TUser>(jwtOption);
    }

    public async Task<IResult> SignUpAsync(TUser user, CancellationToken cancellationToken = default)
    {
        var erros = new List<string>();
        if (string.IsNullOrWhiteSpace(user.EmailAddress))
        {
            erros.Add("Email address is required!");
        }

        if (string.IsNullOrWhiteSpace(user.HashedPassword))
        {
            erros.Add("Password is required!");
        }

        if (erros.Count > 0)
        {
            return Result.Failure(errors: erros);
        }

        var existUser = await MongoUser.Find(x => x.EmailAddress == user.EmailAddress).SingleOrDefaultAsync(cancellationToken);
        if (existUser is not null)
        {
            return Result.Failure("User already exists!");
        }

        user.HashedPassword = user.HashedPassword.HashPassword();
        await MongoUser.InsertOneAsync(user, cancellationToken: cancellationToken);
        return Result<TUser>.Success(user);
    }

    public async Task<Result<SignInResult>> SignInAsync(string email, string password, CancellationToken cancellationToken = default(CancellationToken))
    {
        var existUser = await MongoUser.Find<TUser>(x => x.EmailAddress == email).SingleOrDefaultAsync(cancellationToken);

        if (existUser is null)
        {
            return Result<SignInResult>.Failure(message: "User not found!");

        }

        var verifyPassword = PasswordHashHelper.VerifyHashPassword(password, existUser.HashedPassword);

        if (!verifyPassword)
        {
            return Result<SignInResult>.Failure(message: "Invalid password!");

        }


        if (_jwtHelper is not null)
        {
            var result = new SignInResult
            {
                Token = new Token
                {
                    AccessToken = _jwtHelper.GenerateToken(existUser),
                    AccessExp = DateTime.Now.AddMinutes(10),
                    RefreshToken = _jwtHelper.GenerateRefreshToken(),
                    RefreshExp = DateTime.Now.AddMinutes(15)
                }
            };


            var existUserToken = await UserToken
                .Find(x => x.UserId == existUser.Id)
                .SingleOrDefaultAsync(cancellationToken);

            if (existUserToken is not null)
            {

                existUserToken.Access = result.Token.AccessToken;
                existUserToken.AccessExp = result.Token.AccessExp;
                existUserToken.Refresh = result.Token.RefreshToken;
                existUserToken.RefreshExp = result.Token.RefreshExp;

                await UserToken.FindOneAndReplaceAsync(x => x.UserId == existUser.Id, existUserToken);

            }
            else
            {
                await UserToken
                    .InsertOneAsync(new UserToken
                    {
                        UserId = existUser.Id,
                        Access = result.Token.AccessToken,
                        AccessExp = result.Token.AccessExp,
                        Refresh = result.Token.RefreshToken,
                        RefreshExp = result.Token.RefreshExp
                    }, cancellationToken: cancellationToken);
            }


            return Result<SignInResult>.Success(data: result);
        }

        return Result<SignInResult>.Success(data: null);


    }

    public async Task<IResult> SignOutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var userToken = await UserToken
            .Find(x => x.Refresh == refreshToken)
            .SingleOrDefaultAsync();

        if (userToken is null)
        {
            return Result.Failure("User not found!");
        }

        await UserToken
             .FindOneAndDeleteAsync(x => x.Refresh == refreshToken, cancellationToken: cancellationToken);

        return Result.Success();
    }

}
