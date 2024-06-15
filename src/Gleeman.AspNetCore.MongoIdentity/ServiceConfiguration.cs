using Gleeman.AspNetCore.MongoIdentity.Managers;
using Gleeman.AspNetCore.MongoIdentity.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Gleeman.AspNetCore.MongoIdentity;

public static class ServiceConfiguration
{
    public static IServiceCollection AddMongoIdentity(this IServiceCollection services, Action<MongoOption> option)
    {
        services.AddSingleton<MongoOption>(sp =>
        {
            option.Invoke(sp.GetRequiredService<IOptions<MongoOption>>().Value);
            return sp.GetRequiredService<IOptions<MongoOption>>().Value;
        });

        services.AddScoped(typeof(MongoUserManager<>));
        services.AddScoped(typeof(MongoRoleManager<>));

        return services;

    }

    public static IServiceCollection AddJwt(this IServiceCollection services, Action<JwtOption> jwtOption)
    {
        JwtOption _jwtOption = new();
        if (jwtOption != null)
        {
            services.AddSingleton<JwtOption>(sp =>
            {
                _jwtOption = sp.GetRequiredService<IOptions<JwtOption>>().Value;
                jwtOption.Invoke(_jwtOption);
                return _jwtOption;
            });
        }

        services.AddAuthentication(scheme =>
        {
            scheme.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            scheme.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(conf =>
        {
            conf.SaveToken = true;
            conf.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = _jwtOption.Audience is null ? false : true,
                ValidateIssuer = _jwtOption.Issuer is null ? false : true,
                ValidateIssuerSigningKey = _jwtOption.SecretKey is null ? false : true,

                ValidAudience = _jwtOption.Audience,
                ValidIssuer = _jwtOption.Issuer,
                IssuerSigningKey = _jwtOption.SecretKey is null ? null : new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOption.SecretKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }

}
