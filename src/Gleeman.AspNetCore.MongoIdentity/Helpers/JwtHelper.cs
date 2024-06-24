using Gleeman.AspNetCore.MongoIdentity.Models;
using Gleeman.AspNetCore.MongoIdentity.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Gleeman.AspNetCore.MongoIdentity.Helpers;

public class JwtHelper<TUser>
    where TUser : MongoIdentityUser
{

    private readonly JwtOption _jwtOption;
    private readonly IMongoCollection<UserRole> _userRole;
    private readonly IMongoCollection<MongoIdentityRole> _role;
    public JwtHelper(JwtOption jwtOption, MongoOption mongoOption)
    {
        var client = new MongoClient(mongoOption.ConnectionString);
        var database = client.GetDatabase(mongoOption.DatabaseName);
        _userRole = database.GetCollection<UserRole>(nameof(UserRole));
        _role = database.GetCollection<MongoIdentityRole>("Role");
        _jwtOption = jwtOption;
    }

    public async Task<string> GenerateToken(TUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOption.SecretKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.EmailAddress)
        };

        if(!string.IsNullOrWhiteSpace(user.UserName))
        {
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
        }

        if(!string.IsNullOrWhiteSpace(user.PhoneNumber))
        {
            claims.Add(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));
        }

        var userRoles = await _userRole.Find(x => x.UserId == user.Id).ToListAsync();


        if (userRoles.Count > 0)
        {
            foreach (var userRole in userRoles)
            {
                var existRole = await _role.Find(x => x.Id == userRole.RoleId).SingleOrDefaultAsync();

                if (existRole != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, existRole.RoleName));
                }
            }
        }
     

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtOption.Issuer,
            Audience = _jwtOption.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddMinutes(10),
            SigningCredentials = credentials
        };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }


}
