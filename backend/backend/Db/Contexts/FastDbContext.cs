using Microsoft.EntityFrameworkCore;

namespace backend.Db.Contexts;

public class FastDbContext : GeoDbContext
{
    public FastDbContext(DbContextOptions<FastDbContext> options) : base(options)
    {
    }
}