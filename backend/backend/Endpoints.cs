using backend.Db.Seed;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend;

public static class Endpoints
{
    public static void MapOtusEndpoints(this WebApplication app)
    {
        app.MapPut("/seed", async ([FromServices] ISeedDb seed) =>
        {
            await seed.Seed();
            return Results.Ok();
        });

        app.MapGet("/fast/logistic-centers", async ([FromServices] ILogisticCenterService logisticCenterService) =>
        {
            var logisticCenters = await logisticCenterService.GetFastLogisticCentersAsync();
            return Results.Ok(logisticCenters);
        });

        app.MapGet("/slow/logistic-centers", async ([FromServices] ILogisticCenterService logisticCenterService) =>
        {
            var logisticCenters = await logisticCenterService.GetSlowLogisticCentersAsync();
            return Results.Ok(logisticCenters);
        });

        app.MapGet("/slow/logistic-centers/{logisticCenterId}/couriers",
            async (int logisticCenterId, [FromServices] ICourierService courierService) =>
            {
                var couriers = await courierService.GetCouriersFromSlowDbByLogisticCenterIdAsync(logisticCenterId);
                return Results.Ok(couriers);
            });

        app.MapGet("/fast/logistic-centers/{logisticCenterId}/couriers",
            async (int logisticCenterId, [FromServices] ICourierService courierService) =>
            {
                var couriers = await courierService.GetCouriersByLogisticCenterIdAsync(logisticCenterId);
                return Results.Ok(couriers);
            });

        app.MapGet("/fast/points-near-point", async (string target, [FromServices] IPostgisService postgisService) =>
        {
            var points = await postgisService.GetPointsInPolygonFast(target);
            return Results.Ok(points);
        });

        app.MapGet("/slow/points-near-point", async (string target, [FromServices] IPostgisService postgisService) =>
        {
            var points = await postgisService.GetPointsInPolygonSlow(target);
            return Results.Ok(points);
        });

        app.MapPost("/fast/logistic-centers/{logisticCenterId}/first-10-points", async (int logisticCenterId,
            [FromBody] DateRange dateRange, [FromServices] IPostgisService postgisService) =>
        {
            var points =
                await postgisService.GetFirst10PointsFromLogisticCenter(logisticCenterId, dateRange.StartTime.ToUniversalTime(),
                    dateRange.EndTime.ToUniversalTime());
            return Results.Ok(points);
        });

        app.MapPost("/slow/logistic-centers/{logisticCenterId}/first-10-points", async (int logisticCenterId,
            [FromBody] DateRange dateRange, [FromServices] IPostgisService postgisService) =>
        {
            var points =
                await postgisService.GetFirst10PointsFromLogisticCenterSlow(logisticCenterId, dateRange.StartTime.ToUniversalTime(),
                    dateRange.EndTime.ToUniversalTime());
            return Results.Ok(points);
        });
        app.MapGet("/fast/points-in-rectangle",
            async (double x1, double y1, double x2, double y2, [FromServices] IPostgisService postgisService) =>
            {
                var points = await postgisService.GetPointsInRectangle(x1, y1, x2, y2);
                return Results.Ok(points);
            });

        app.MapGet("/slow/points-in-rectangle",
            async (double x1, double y1, double x2, double y2, [FromServices] IPostgisService postgisService) =>
            {
                var points = await postgisService.GetPointsInRectangleSlow(x1, y1, x2, y2);
                return Results.Ok(points);
            });
    }
}