using backend.Db.Contexts;
using Microsoft.EntityFrameworkCore;

namespace backend.Db.Seed;

public class SeedDb : ISeedDb
{
    private readonly FastDbContext _fastDbContext;
    private readonly SlowDbContext _slowDbContext;

    public SeedDb(FastDbContext fastDbContext, SlowDbContext slowDbContext)
    {
        _fastDbContext = fastDbContext;
        _slowDbContext = slowDbContext;
    }

    public async Task Seed()
    {
        var scriptPath = "Db/Seed/seedFast.sql";
        var script = await File.ReadAllTextAsync(scriptPath);
        var fast = _fastDbContext.Database.ExecuteSqlRawAsync(script);
        
        scriptPath = "Db/Seed/seedSlow.sql";
        script = await File.ReadAllTextAsync(scriptPath);
        var slow = _slowDbContext.Database.ExecuteSqlRawAsync(script);
        await Task.WhenAll(fast, slow);

        Console.WriteLine("Database seeded successfully.");
    }
}