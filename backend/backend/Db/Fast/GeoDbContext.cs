using backend.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Db.Fast;

public class GeoDbContext : DbContext
{
    public GeoDbContext(DbContextOptions options) : base(options) { }

    public DbSet<LogisticCenter> LogisticCenters { get; set; }
    public DbSet<Courier> Couriers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Delivery> Deliveries { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Delivery>()
            .Property(d => d.Point)
            .HasColumnType("geometry(Point, 4326)");
        
        modelBuilder.Entity<LogisticCenter>()
            .Property(d => d.Location)
            .HasColumnType("geometry(Point, 4326)");
        modelBuilder.HasPostgresExtension("postgis");
    }
}