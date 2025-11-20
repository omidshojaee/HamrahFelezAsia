using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using HamrahFelez.Repository.DataAccess;
using HamrahFelez.Utilities.Attributes;
using HamrahFelez.Utilities.Helpers;

var builder = WebApplication.CreateBuilder(args);

// JWT settings
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

var keyString = jwtSettings["Key"]
                ?? throw new InvalidOperationException("Jwt Key is not configured.");
var issuer = jwtSettings["Issuer"]
             ?? throw new InvalidOperationException("Jwt Issuer is not configured.");
var audience = jwtSettings["Audience"]
               ?? throw new InvalidOperationException("Jwt Audience is not configured.");

var keyBytes = Encoding.UTF8.GetBytes(keyString);

// Services
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// Swagger + JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hamrah Felez Asia Web API",
        Version = "v1"
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

// Jwt helper
builder.Services.AddScoped<Jwt>();

// Use literal scheme name instead of JwtBearerDefaults
const string jwtScheme = "Bearer";

builder.Services
    .AddAuthentication(jwtScheme)
    .AddJwtBearer(jwtScheme, options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

// HTTP pipeline
app.UseSwagger();
app.UseSwaggerUI();

// Select DB based on [UseProductionDb] attribute
app.Use(async (ctx, next) =>
{
    var endpoint = ctx.GetEndpoint();
    var useProduction = endpoint?.Metadata.GetMetadata<UseProductionDbAttribute>() != null;

    var cfg = ctx.RequestServices.GetRequiredService<IConfiguration>();
    var cs = cfg.GetConnectionString(useProduction ? "DbMain" : "DbMain_Develop");

    DataAccessManager.ConnectionString = cs;

    await next();
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
