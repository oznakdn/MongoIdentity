using Gleeman.AspNetCore.MongoIdentity;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
