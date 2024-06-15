using Gleeman.AspNetCore.MongoIdentity.Models;
using Gleeman.AspNetCore.MongoIdentity.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Gleeman.AspNetCore.MongoIdentity.Helpers;

public class JwtHelper<TUser>
    where TUser : MongoIdentityUser
{

    private readonly JwtOption _jwtOption;
    public JwtHelper(JwtOption jwtOption)
    {
        _jwtOption = jwtOption;
    }

    public string GenerateToken(TUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOption.SecretKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.EmailAddress)
        };


        

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
