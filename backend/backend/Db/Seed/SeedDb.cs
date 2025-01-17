using backend.Db.Entities;
using backend.Db.Fast;
using Microsoft.EntityFrameworkCore;

namespace backend.Db.Seed;

public class SeedDb : ISeedDb
{
    private readonly FastDbContext _fastDbContext;
    private readonly SlowDbContext _slowDbContext;
    private readonly DateTimeGenerator _dateTimeGenerator = new();

    public SeedDb(FastDbContext fastDbContext, SlowDbContext slowDbContext)
    {
        _fastDbContext = fastDbContext;
        _slowDbContext = slowDbContext;
    }

    public async Task Seed()
    {
        await ClearAllTables();
        Console.WriteLine("All tables cleared.");

        var centers = LogisticCenterGenerator.GenerateLogisticCenters(10, 54.7818, 32.0401, 50);
        _fastDbContext.LogisticCenters.AddRange(centers);
        _slowDbContext.LogisticCenters.AddRange(centers);
        Console.WriteLine("Logistic centers added.");

        var couriers = new List<Courier>();
        var orders = new List<Order>();

        foreach (var center in centers)
        {
            var generatedCouriers = GenerateCouriersWithOrders(center);
            couriers.AddRange(generatedCouriers);
        }

        _fastDbContext.Couriers.AddRange(couriers);
        _slowDbContext.Couriers.AddRange(couriers);
        Console.WriteLine("Couriers added.");

        foreach (var courier in couriers)
        {
            var generatedOrders = GenerateOrders(courier);
            orders.AddRange(generatedOrders);
        }

        _fastDbContext.Orders.AddRange(orders);
        _slowDbContext.Orders.AddRange(orders);
        Console.WriteLine("Orders added.");

        await _fastDbContext.SaveChangesAsync();
        await _slowDbContext.SaveChangesAsync();
        Console.WriteLine("Logistic centers, couriers, and orders saved.");

        var fastOrders = await _fastDbContext.Orders.ToListAsync();
        await GenerateDeliveries(fastOrders, _fastDbContext);
        var slowOrders = await _slowDbContext.Orders.ToListAsync();
        await GenerateDeliveries(slowOrders, _slowDbContext);
        Console.WriteLine("Deliveries generated and saved.");
    }

    private async Task GenerateDeliveries(List<Order> orders, GeoDbContext context)
    {
        var centersWithCouriers = await context.Couriers
            .Include(c => c.LogisticCenter)
            .Select(c => new { c.Id, c.LogisticCenter.Location })
            .ToDictionaryAsync(arg => arg.Id, arg => arg.Location);
        
        var deliveries = new List<Delivery>();

        foreach (var order in orders)
        {
            var points = PointSeriesGenerator.GeneratePointSeries(
                centersWithCouriers[order.CourierId],
                50,
                150,
                10,
                50);
            deliveries.AddRange(points.Select((p, i) => new Delivery
            {
                OrderId = order.Id,
                Point = p,
                DeliveryTimestamp = order.OrderTimestamp + TimeSpan.FromMinutes(i),
                Order = order,
                Year = order.OrderTimestamp.Year,
            }));
            context.Deliveries.AddRange(deliveries);
            await context.SaveChangesAsync();
            Console.WriteLine("Deliveries added.");
            deliveries.Clear();
        }
    }

    private async Task ClearAllTables()
    {
        await TruncateTableAsync(_fastDbContext, "deliveries");
        await TruncateTableAsync(_fastDbContext, "orders");
        await TruncateTableAsync(_fastDbContext, "couriers");
        await TruncateTableAsync(_fastDbContext, "logistic_centers");
        await TruncateTableAsync(_slowDbContext, "deliveries");
        await TruncateTableAsync(_slowDbContext, "orders");
        await TruncateTableAsync(_slowDbContext, "couriers");
        await TruncateTableAsync(_slowDbContext, "logistic_centers");
    }

    private List<Courier> GenerateCouriersWithOrders(LogisticCenter center)
    {
        var couriers = new List<Courier>();

        for (int i = 0; i < 10; i++)
        {
            Courier courier = new()
            {
                LogisticCenterId = center.Id,
                Name = $"Courier {i} on center {center.Id}",
                LogisticCenter = center
            };

            couriers.Add(courier);
        }

        return couriers;
    }

    private List<Order> GenerateOrders(Courier courier)
    {
        var orders = new List<Order>();

        for (int i = 0; i < 10; i++)
        {
            orders.Add(new Order
            {
                CourierId = courier.Id,
                OrderTimestamp = _dateTimeGenerator.GenerateRandomDateTime().ToUniversalTime(),
                Courier = courier
            });
        }
       
        /*
        for (int i = 0; i < 50; i++)
        {
            orders.Add(new Order
            {
                CourierId = courier.Id,
                OrderTimestamp = _dateTimeGenerator.GenerateRandomDateTime().ToUniversalTime(),
                Courier = courier
            });
        }
        */

        return orders;
    }

    private Task TruncateTableAsync(DbContext context, string tableName)
    {
        return context.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE {tableName} RESTART IDENTITY CASCADE");
    }
    
    private async Task ClearTable<T>(DbContext context) where T : class
    {
        var dbSet = context.Set<T>();
        context.RemoveRange(dbSet);
        await context.SaveChangesAsync();
        Console.WriteLine($"Table {typeof(T).Name} cleared.");
    }
}