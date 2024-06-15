namespace Gleeman.AspNetCore.MongoIdentity.Results;

public class SignInResult
{
    public Token Token { get; set; }
}


public class Token
{
    public string AccessToken { get; set; }
    public DateTime AccessExp { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshExp { get; set; }

}