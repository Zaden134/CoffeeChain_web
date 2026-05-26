using CoffeeChainManagement.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Day la diem khoi dong backend: dang ky controller, swagger, CORS va service nen.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "Frontend",
        policy => policy
            .WithOrigins(
                builder.Configuration["Frontend:Url"] ?? "http://localhost:4200",
                "http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod());
});
builder.Services.AddInfrastructure();

var app = builder.Build();

// Swagger giup team test API nhanh trong giai doan setup va phat trien.
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("Frontend");
app.UseAuthorization();
app.MapControllers();

app.MapGet(
    "/health",
    () => Results.Ok(new
    {
        status = "healthy",
        service = "coffee-chain-api",
        utc = DateTime.UtcNow
    }));

app.Run();
