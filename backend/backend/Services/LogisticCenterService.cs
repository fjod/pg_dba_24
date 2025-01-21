using backend.Db.Contexts;
using backend.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class LogisticCenterService : ILogisticCenterService
{
    private readonly GeoDbContext _fastDbContext;
    private readonly SlowDbContext _slowDbContext;

    public LogisticCenterService(FastDbContext fastDbContext, SlowDbContext slowDbContext)
    {
        _fastDbContext = fastDbContext;
        _slowDbContext = slowDbContext;
    }

    public async Task<IEnumerable<LogisticCenter>> GetFastLogisticCentersAsync()
    {
        return await _fastDbContext.LogisticCenters.ToListAsync();
    }

    public async Task<IEnumerable<LogisticCenter>> GetSlowLogisticCentersAsync()
    {
        return await _slowDbContext.LogisticCenters.ToListAsync();
    }
}