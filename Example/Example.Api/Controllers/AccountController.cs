using Example.Api.Dtos;
using Example.Api.Models;
using Gleeman.AspNetCore.MongoIdentity.Managers;
using Microsoft.AspNetCore.Mvc;

namespace Example.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly MongoUserManager<AppUser> _userManager;

    public AccountController(MongoUserManager<AppUser> userManager)
    {
        _userManager = userManager;
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
}
