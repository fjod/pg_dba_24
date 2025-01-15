using Microsoft.EntityFrameworkCore;

namespace backend.Db.Fast;

public class FastDbContext : GeoDbContext
{
    public FastDbContext(DbContextOptions<FastDbContext> options) : base(options)
    {
    }
}