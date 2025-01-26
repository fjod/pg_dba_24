// File: backend/Services/CourierService.cs
using backend.Db.Contexts;
using backend.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class CourierService : ICourierService
{
    private readonly FastDbContext _fastDbContext;
    private readonly SlowDbContext _slowDbContext;

    public CourierService(FastDbContext fastDbContext, SlowDbContext slowDbContext)
    {
        _fastDbContext = fastDbContext;
        _slowDbContext = slowDbContext;
    }

    public async Task<IEnumerable<Courier>> GetCouriersByLogisticCenterIdAsync(int logisticCenterId)
    {
        return await _fastDbContext.Couriers
            .Where(c => c.LogisticCenterId == logisticCenterId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Courier>> GetCouriersFromSlowDbByLogisticCenterIdAsync(int logisticCenterId)
    {
        return await _slowDbContext.Couriers
            .Where(c => c.LogisticCenterId == logisticCenterId)
            .ToListAsync();
    }
}