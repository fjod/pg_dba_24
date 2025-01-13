using Microsoft.EntityFrameworkCore;

namespace backend.Db.Fast;

public class FastDbContext : GeoDbContext
{
    public FastDbContext(DbContextOptions<GeoDbContext> options) : base(options)
    {
    }
}