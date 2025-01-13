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
        
        var centers = LogisticCenterGenerator.GenerateLogisticCenters(10, 54.7818, 32.0401, 50);
        foreach (var center in centers)
        {
            GenerateCouriers(center);
            _fastDbContext.LogisticCenters.Add(center);
            _slowDbContext.LogisticCenters.Add(center);
        }
        
        await _fastDbContext.SaveChangesAsync();
        await _slowDbContext.SaveChangesAsync();

        var fastOrders = await _fastDbContext.Orders.AsNoTracking().ToListAsync();
        await GenerateDeliveries(fastOrders, _fastDbContext);
        var slowOrders = await _slowDbContext.Orders.AsNoTracking().ToListAsync();
        await GenerateDeliveries(slowOrders, _slowDbContext);
    }

    private async Task GenerateDeliveries(List<Order> slowOrders, GeoDbContext context)
    {
        var centersList = await _fastDbContext
            .Orders.Include(o=>o.Courier)
            .ThenInclude(c=>c.LogisticCenter)
            .Select(o => new {o.Id, o.Courier.LogisticCenter.Location})
            .AsNoTracking()
            .ToDictionaryAsync(arg => arg.Id, arg => arg.Location);
        foreach (var order in slowOrders)
        {
            var points = PointSeriesGenerator.GeneratePointSeries(
                centersList[order.Id], 
                50, 
                150,
                1,
                10);
            var deliveries = points.Select((p, i) => new Delivery
            {
                OrderId = order.Id,
                Point = p,
                DeliveryTimestamp = order.OrderTimestamp + TimeSpan.FromMinutes(i),
                Order = order,
                YearMonth = order.OrderTimestamp.Year * 100 + order.OrderTimestamp.Month,
            }).ToList();
            context.Deliveries.AddRange(deliveries);
            await context.SaveChangesAsync();
        }
    }

    private async Task ClearAllTables()
    {
        await ClearTable<Delivery>(_fastDbContext);
        await ClearTable<Order>(_fastDbContext);
        await ClearTable<Courier>(_fastDbContext);
        await ClearTable<LogisticCenter>(_fastDbContext);
        
        await ClearTable<Delivery>(_slowDbContext);
        await ClearTable<Order>(_slowDbContext);
        await ClearTable<Courier>(_slowDbContext);
        await ClearTable<LogisticCenter>(_slowDbContext);
    }

    private void GenerateCouriers(LogisticCenter center)
    {
        for (int i = 0; i < 10; i++)
        {
            Courier courier = new()
            {
                LogisticCenterId = center.Id,
                Name = $"Courier {i} on center {center.Id}",
                LogisticCenter = center
            };

            GenerateOrders(courier);
            _fastDbContext.Couriers.Add(courier);
            _slowDbContext.Couriers.Add(courier);
        }
    }

    private void GenerateOrders(Courier courier)
    {
        for (int i = 0; i < 100; i++)
        {
            Order o = new Order
            {
                CourierId = courier.Id,
                OrderTimestamp = _dateTimeGenerator.GenerateRandomDateTime(),
                Courier = courier
            };
            _fastDbContext.Orders.Add(o);
            _slowDbContext.Orders.Add(o);
        }
    }
    
    public async Task ClearTable<T>(DbContext context) where T : class
    {
        var dbSet = context.Set<T>();
        context.RemoveRange(dbSet);
        await context.SaveChangesAsync();
    }
}