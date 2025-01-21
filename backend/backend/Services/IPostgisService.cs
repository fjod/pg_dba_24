using backend.Models;

namespace backend.Services;

public interface IPostgisService
{
    /// <summary>
    /// Запрос 1 - точки рядом с заданной точкой
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    Task<IEnumerable<PointWithOrder>> GetPointsInPolygonFast(string target);
    
    Task<IEnumerable<PointWithOrder>> GetPointsInPolygonSlow(string target);

    /// <summary>
    /// Запрос 2 - первые 10 точек заказа рядом с логистическим центром
    /// </summary>
    /// <param name="logisticCenterId"></param>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <returns></returns>
    public Task<IEnumerable<PointWithOrder>> GetFirst10PointsFromLogisticCenter(int logisticCenterId,
        DateTime startTime, DateTime endTime);

    public Task<IEnumerable<PointWithOrder>> GetFirst10PointsFromLogisticCenterSlow(int logisticCenterId,
        DateTime startTime, DateTime endTime);
    
    /// <summary>
    /// Запрос 3 - точки в прямоугольнике
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <returns></returns>
    public Task<IEnumerable<PointWithOrder>> GetPointsInRectangle(double x1, double y1, double x2, double y2);
    
    public Task<IEnumerable<PointWithOrder>> GetPointsInRectangleSlow(double x1, double y1, double x2, double y2);
}