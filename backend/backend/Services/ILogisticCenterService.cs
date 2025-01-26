using backend.Db.Entities;

namespace backend.Services;

public interface ILogisticCenterService
{
    Task<IEnumerable<LogisticCenter>> GetFastLogisticCentersAsync();
    
    Task<IEnumerable<LogisticCenter>> GetSlowLogisticCentersAsync();
}