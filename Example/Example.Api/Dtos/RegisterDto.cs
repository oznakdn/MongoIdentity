namespace Example.Api.Dtos;

public record RegisterDto(string FirstName, string LastName, string Email, string Password, string? UserName = null, string? PhoneNumber = null);

