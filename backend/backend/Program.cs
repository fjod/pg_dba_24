using backend.Db.Fast;
using backend.Db.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FastDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Fast"),
        o => o.UseNetTopologySuite())
        .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine, LogLevel.Information));

builder.Services.AddDbContext<SlowDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Slow"), 
            o => o.UseNetTopologySuite())
      .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine, LogLevel.Information));

builder.Services.AddScoped<ISeedDb, SeedDb>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapPut("/seed", async (ISeedDb seed) =>
{
    await seed.Seed();
    return Results.Ok();
});

app.Run();
