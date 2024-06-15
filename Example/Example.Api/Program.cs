using Gleeman.AspNetCore.MongoIdentity;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMongoIdentity(opt =>
{
    opt.ConnectionString = "mongodb://localhost:27017";
    opt.DatabaseName = "ExampleDB";
}, jwt =>
{
    jwt.SecretKey = "f5bf2a7a-c397-4ef9-9bec-bd1255d7b26e";
    jwt.Audience = "this is audience";
    jwt.Issuer = "this is issuer";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
