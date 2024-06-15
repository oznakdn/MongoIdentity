# Gleeman.AspNetCore.MongoIdentity

#### Install packages
`CLI`
```csharp
dotnet add package Gleeman.AspNetCore.MongoIdentity
```

#### Program.cs without JwtBearer
```csharp
builder.Services.AddMongoIdentity(opt =>
{
    opt.ConnectionString = "mongodb://localhost:27017";
    opt.DatabaseName = "ExampleDB";
});

```

#### Program.cs with JwtBearer

```csharp
builder.Services.AddMongoIdentity(opt =>
{
    opt.ConnectionString = "mongodb://localhost:27017";
    opt.DatabaseName = "ExampleDB";

}).AddJwt(opt =>
{
    opt.SecretKey = "f5bf2a7a-c397-4ef9-9bec-bd1255d7b26e";
    opt.Audience = "this is audience";
    opt.Issuer = "this is issuer";
});


app.UseAuthentication();
app.UseAuthorization();

```


<a href="https://github.com/oznakdn/MongoIdentity/tree/master/Example/Example.Api">Example</a>

