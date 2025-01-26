using backend.Db.Entities;

namespace backend.Services;

public interface ICourierService
{
    Task<IEnumerable<Courier>> GetCouriersByLogisticCenterIdAsync(int logisticCenterId);
    Task<IEnumerable<Courier>> GetCouriersFromSlowDbByLogisticCenterIdAsync(int logisticCenterId);
}