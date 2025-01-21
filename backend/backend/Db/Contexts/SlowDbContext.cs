using Microsoft.EntityFrameworkCore;

namespace backend.Db.Contexts;

public class SlowDbContext : GeoDbContext
{
    public SlowDbContext(DbContextOptions<SlowDbContext> options) : base(options)
    {
    }
}