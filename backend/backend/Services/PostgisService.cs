using backend.Models;
using backend.Db.Contexts;
using NetTopologySuite.Geometries;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class PostgisService : IPostgisService
    {
        private readonly FastDbContext _fastDbContext;
        private readonly SlowDbContext _slowDbContext;

        public PostgisService(FastDbContext fastDbContext, SlowDbContext slowDbContext)
        {
            _fastDbContext = fastDbContext;
            _slowDbContext = slowDbContext;
        }

        public Task<IEnumerable<PointWithOrder>> GetPointsInPolygonFast(string target)
        {
            return PointsInPolygon(target, _fastDbContext);
        }
        public Task<IEnumerable<PointWithOrder>> GetPointsInPolygonSlow(string target)
        {
            return PointsInPolygon(target, _slowDbContext);
        }

        private async Task<IEnumerable<PointWithOrder>> PointsInPolygon(string target, GeoDbContext context)
        {
            var coordinates = target.Trim('(', ')').Split(' ');
            var longitude = double.Parse(coordinates[0]);
            var latitude = double.Parse(coordinates[1]);
            var point = new Point(longitude, latitude) { SRID = 4326 };

            var result = await context.Deliveries
                .Where(d => d.Point.IsWithinDistance(point, 0.5))
                .OrderBy(d => d.DeliveryTimestamp)
                .Take(100)
                .Select(d => new PointWithOrder
                {
                    Coordinates = d.Point.ToString(),
                    OrderId = d.OrderId
                })
                .ToListAsync();

            return result;
        }
        
        public Task<IEnumerable<PointWithOrder>> GetFirst10PointsFromLogisticCenter(int logisticCenterId, DateTime startTime, DateTime endTime)
        {
            return First10PointsFromLogisticCenter(logisticCenterId, startTime, endTime, _fastDbContext);
        }

        public Task<IEnumerable<PointWithOrder>> GetFirst10PointsFromLogisticCenterSlow(int logisticCenterId, DateTime startTime, DateTime endTime)
        {
            return First10PointsFromLogisticCenter(logisticCenterId, startTime, endTime, _slowDbContext);
        }

        private async Task<IEnumerable<PointWithOrder>> First10PointsFromLogisticCenter(int logisticCenterId, DateTime startTime, DateTime endTime, GeoDbContext context)
        {
            var result = await context.Deliveries
                .FromSqlInterpolated($@"
            WITH ranked_deliveries AS (
                SELECT
                    d.id,
                    d.order_id,
                    d.point,
                    d.delivery_timestamp,
                    ROW_NUMBER() OVER (PARTITION BY d.order_id ORDER BY d.delivery_timestamp) AS rn
                FROM deliveries d
                         JOIN orders o ON o.id = d.order_id
                         JOIN couriers c ON c.id = o.courier_id
                         JOIN logistic_centers lc ON lc.id = c.logistic_center_id
                WHERE lc.id = {logisticCenterId}
                  AND ST_DWithin(lc.location, d.point, 0.1)
                  AND d.delivery_timestamp >= {startTime}
                  AND d.delivery_timestamp < {endTime}
            )
            SELECT
                point,
                order_id
            FROM ranked_deliveries
            WHERE rn <= 10
            ORDER BY order_id, delivery_timestamp
        ")
                .Select(d => new PointWithOrder
                {
                    Coordinates = d.Point.ToString(),
                    OrderId = d.OrderId
                })
                .ToListAsync();

            return result;
        }
        
        public Task<IEnumerable<PointWithOrder>> GetPointsInRectangle(double x1, double y1, double x2, double y2)
        {
            return PointsInRectangle(x1, y1, x2, y2, _fastDbContext);
        }
        
        public Task<IEnumerable<PointWithOrder>> GetPointsInRectangleSlow(double x1, double y1, double x2, double y2)
        {
            return PointsInRectangle(x1, y1, x2, y2, _slowDbContext);
        }
        
        private async Task<IEnumerable<PointWithOrder>> PointsInRectangle(double x1, double y1, double x2, double y2, GeoDbContext context)
        {
            var result = await context.Deliveries
                .FromSqlInterpolated($@"
            SELECT point, order_id
            FROM deliveries
            WHERE ST_Intersects(
                      point,
                      ST_MakeEnvelope({x1}, {y1}, {x2}, {y2}, 4326)
                  )
            ORDER BY delivery_timestamp
            LIMIT 100
        ")
                .Select(d => new PointWithOrder
                {
                    Coordinates = d.Point.ToString(),
                    OrderId = d.OrderId
                })
                .ToListAsync();

            return result;
        }
    }
    
    
}