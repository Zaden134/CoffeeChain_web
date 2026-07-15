using System.Security.Claims;
using System.Text;
using CoffeeChainManagement.Infrastructure;
using CoffeeChainManagement.Infrastructure.Auth;
using CoffeeChainManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});
var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("Jwt configuration is missing.");

builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole(options =>
{
    options.IncludeScopes = true;
    options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
});

// Day la diem khoi dong backend: dang ky controller, swagger, CORS va service nen.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Nhap JWT token theo dinh dang: Bearer {token}"
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
});
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
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var serverSession = context.HttpContext.RequestServices.GetRequiredService<ServerSessionMarker>();
                var tokenSessionId = context.Principal?.FindFirstValue(ServerSessionMarker.ClaimType);

                if (!string.Equals(tokenSessionId, serverSession.SessionId, StringComparison.Ordinal))
                {
                    context.Fail("Server restarted. Please sign in again.");
                }

                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks();

var app = builder.Build();

await app.Services.InitializeDatabaseAsync();

// Swagger giup team test API nhanh trong giai doan setup va phat trien.
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("Frontend");
app.UseAuthentication();
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

app.MapGet(
    "/health/db",
    async (CoffeeChainDbContext dbContext, CancellationToken cancellationToken) =>
    {
        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
        return canConnect
            ? Results.Ok(new { status = "healthy", dependency = "postgresql", utc = DateTime.UtcNow })
            : Results.Problem("PostgreSQL connection failed.", statusCode: StatusCodes.Status503ServiceUnavailable);
    });

app.MapGet(
    "/health/info",
    () => Results.Ok(new
    {
        service = "coffee-chain-api",
        environment = app.Environment.EnvironmentName,
        utc = DateTime.UtcNow
    }));

app.Run();
