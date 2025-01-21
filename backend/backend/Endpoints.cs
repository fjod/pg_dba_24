using backend.Db.Seed;
using backend.Services;

namespace backend;

public static class Endpoints
{
    public static void MapOtusEndpoints(this WebApplication app)
    {
        app.MapPut("/seed", async (ISeedDb seed) =>
        {
            await seed.Seed();
            return Results.Ok();
        });

        app.MapGet("/fast/logistic-centers", async (ILogisticCenterService logisticCenterService) =>
        {
            var logisticCenters = await logisticCenterService.GetFastLogisticCentersAsync();
            return Results.Ok(logisticCenters);
        });

        app.MapGet("/slow/logistic-centers", async (ILogisticCenterService logisticCenterService) =>
        {
            var logisticCenters = await logisticCenterService.GetSlowLogisticCentersAsync();
            return Results.Ok(logisticCenters);
        });

        app.MapGet("/slow/logistic-centers/{logisticCenterId}/couriers", async (int logisticCenterId, ICourierService courierService) =>
        {
            var couriers = await courierService.GetCouriersFromSlowDbByLogisticCenterIdAsync(logisticCenterId);
            return Results.Ok(couriers);
        });

        app.MapGet("/fast/logistic-centers/{logisticCenterId}/couriers", async (int logisticCenterId, ICourierService courierService) =>
        {
            var couriers = await courierService.GetCouriersByLogisticCenterIdAsync(logisticCenterId);
            return Results.Ok(couriers);
        });
        
        app.MapGet("/fast/points-in-polygon", async (string target, IPostgisService postgisService) =>
        {
            var points = await postgisService.GetPointsInPolygonFast(target);
            return Results.Ok(points);
        });

        app.MapGet("/slow/points-in-polygon", async (string target, IPostgisService postgisService) =>
        {
            var points = await postgisService.GetPointsInPolygonSlow(target);
            return Results.Ok(points);
        });

        app.MapGet("/fast/logistic-centers/{logisticCenterId}/first-10-points", async (int logisticCenterId, DateTime startTime, DateTime endTime, IPostgisService postgisService) =>
        {
            var points = await postgisService.GetFirst10PointsFromLogisticCenter(logisticCenterId, startTime, endTime);
            return Results.Ok(points);
        });

        app.MapGet("/slow/logistic-centers/{logisticCenterId}/first-10-points", async (int logisticCenterId, DateTime startTime, DateTime endTime, IPostgisService postgisService) =>
        {
            var points = await postgisService.GetFirst10PointsFromLogisticCenterSlow(logisticCenterId, startTime, endTime);
            return Results.Ok(points);
        });

        app.MapGet("/fast/points-in-rectangle", async (double x1, double y1, double x2, double y2, IPostgisService postgisService) =>
        {
            var points = await postgisService.GetPointsInRectangle(x1, y1, x2, y2);
            return Results.Ok(points);
        });

        app.MapGet("/slow/points-in-rectangle", async (double x1, double y1, double x2, double y2, IPostgisService postgisService) =>
        {
            var points = await postgisService.GetPointsInRectangleSlow(x1, y1, x2, y2);
            return Results.Ok(points);
        });
    }
}