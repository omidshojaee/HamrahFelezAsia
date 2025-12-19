using HamrahFelez.Repositories;
using HamrahFelez.Utilities;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Return a unified JSON response when ModelState is invalid
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    e => e.Key,
                    e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray()
                );

            return new BadRequestObjectResult(new
            {
                success = false,
                message = "Your request contains invalid data.",
                errors
            });
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Per-request connection string selector (production vs development DB)
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

app.UseAuthorization();

app.MapControllers();

app.Run();
