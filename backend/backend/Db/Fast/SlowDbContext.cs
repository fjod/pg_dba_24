using Microsoft.EntityFrameworkCore;

namespace backend.Db.Fast;

public class SlowDbContext : GeoDbContext
{
    public SlowDbContext(DbContextOptions<SlowDbContext> options) : base(options)
    {
    }
}