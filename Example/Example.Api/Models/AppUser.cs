using Gleeman.AspNetCore.MongoIdentity.Models;

namespace Example.Api.Models;

public class AppUser : MongoIdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
