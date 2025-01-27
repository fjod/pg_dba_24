using backend;
using backend.Db.Contexts;
using backend.Db.Seed;
using backend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<FastDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Fast"),
            o => o.UseNetTopologySuite().CommandTimeout(600))
        .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine, LogLevel.Information));

builder.Services.AddDbContext<SlowDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Slow"), 
            o => o.UseNetTopologySuite().CommandTimeout(600))
        .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine, LogLevel.Information));

builder.Services.AddScoped<ISeedDb, SeedDb>();
builder.Services.AddScoped<ICourierService, CourierService>();
builder.Services.AddScoped<ILogisticCenterService, LogisticCenterService>();
builder.Services.AddScoped<IPostgisService, PostgisService>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapOtusEndpoints();

app.Run();