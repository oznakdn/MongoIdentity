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
    private readonly MongoUserRoleManager<AppUser, MongoIdentityRole> _userRoleManager;

    public AccountController(MongoUserManager<AppUser> userManager, MongoRoleManager<MongoIdentityRole> roleManager, MongoUserRoleManager<AppUser, MongoIdentityRole> userRoleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userRoleManager = userRoleManager;
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

    [HttpGet]
    public async Task<IActionResult> GetRoles(CancellationToken cancellationToken)
    {
        var result = await _roleManager.GetRolesAsync(cancellationToken);
        return Ok(result);
    }


    [HttpGet]
    public async Task<IActionResult> GetRole(string roleName, CancellationToken cancellationToken)
    {
        var result = await _roleManager.GetRoleByNameAsync(roleName, cancellationToken);
        return Ok(result);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoleById(string id, CancellationToken cancellationToken)
    {
        var result = await _roleManager.GetRoleByIdAsync(id, cancellationToken);
        return Ok(result);
    }


    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto createRole, CancellationToken cancellationToken)
    {
        var result = await _roleManager.CreateAsync(new MongoIdentityRole
        {
            RoleName = createRole.RoleName,
            Description = createRole.Description

        }, cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleDto updateRole, CancellationToken cancellationToken)
    {
        var result = await _roleManager.UpdateAsync(new MongoIdentityRole
        {
            Id = updateRole.Id,
            RoleName = updateRole.RoleName,
            Description = updateRole.Description
        }, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{roleName}")]
    public async Task<IActionResult> DeleteRole(string roleName, CancellationToken cancellationToken)
    {
        var result = await _roleManager.DeleteAsync(roleName, cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Message);

        return BadRequest(result.Message);
    }

    [HttpPost]
    public async Task<IActionResult> AssignRole(string email, string roleName, CancellationToken cancellationToken)
    {
        var userResult = await _userManager.GetUserAsync(email, cancellationToken);
        if(!userResult.IsSuccess)
        {
            return BadRequest(userResult.Message);
        }

        var result = await _userRoleManager.AddRoleToUserAsync(userResult.Value!, roleName, cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Message);

        return BadRequest(result.Message);
    }

    [HttpDelete]
    public async Task<IActionResult>RemoveRoleFromUser(string email, string roleName, CancellationToken cancellationToken)
    {
        var userResult = await _userManager.GetUserAsync(email, cancellationToken);
        if (!userResult.IsSuccess)
        {
            return BadRequest(userResult.Message);
        }

        var result = await _userRoleManager.RemoveRoleFromUserAsync(userResult.Value!, roleName, cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Message);

        return BadRequest(result.Message);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsersInRole(string roleName, CancellationToken cancellationToken)
    {
        var result = await _userRoleManager.GetUsersInRoleAsync(roleName, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Message);
        return Ok(result);
    }

}
