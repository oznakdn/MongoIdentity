using Example.Api.Dtos;
using Example.Api.Models;
using Gleeman.AspNetCore.MongoIdentity.Managers;
using Gleeman.AspNetCore.MongoIdentity.Models;
using Microsoft.AspNetCore.Mvc;

namespace Example.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly MongoUserManager<AppUser> _userManager;
    private readonly MongoRoleManager<MongoIdentityRole> _roleManager;

    public AccountController(MongoUserManager<AppUser> userManager,MongoRoleManager<MongoIdentityRole>roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterDto register, CancellationToken cancellationToken)
    {
        var result = await _userManager.SignUpAsync(new AppUser
        {
            EmailAddress = register.Email,
            FirstName = register.FirstName,
            LastName = register.LastName,
            UserName = register.UserName ?? string.Empty,
            HashedPassword = register.Password,
            PhoneNumber = register.PhoneNumber ?? string.Empty
        }, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password, CancellationToken cancellationToken)
    {
        var result = await _userManager.SignInAsync(email, password, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody]CreateRoleDto createRole,CancellationToken cancellationToken)
    {
        var result = await _roleManager.CreateAsync(new MongoIdentityRole
        {
            RoleName = createRole.RoleName,
            Description = createRole.Description

        },cancellationToken);
        return Ok(result);
    }
}
