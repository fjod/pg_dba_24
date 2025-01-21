using backend;
using backend.Db.Contexts;
using backend.Db.Seed;
using backend.Services;
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
builder.Services.AddScoped<ILogisticCenterService, LogisticCenterService>();
builder.Services.AddScoped<IPostgisService, PostgisService>();

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
app.MapOtusEndpoints();

app.Run();
