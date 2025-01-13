using backend.Db.Entities;

namespace backend.Db.Seed;

using NetTopologySuite.Geometries;
using System;

public class LogisticCenterGenerator
{
    private static readonly Random Random = new Random();

    public static Point GenerateRandomPoint(double latitude, double longitude, double maxDistanceKm)
    {
        double radiusInDegrees = maxDistanceKm / 111.32; // Convert km to degrees
        double u = Random.NextDouble();
        double v = Random.NextDouble();
        double w = radiusInDegrees * Math.Sqrt(u);
        double t = 2 * Math.PI * v;
        double x = w * Math.Cos(t);
        double y = w * Math.Sin(t);

        double newLatitude = latitude + y;
        double newLongitude = longitude + x / Math.Cos(latitude * Math.PI / 180);

        return new Point(newLongitude, newLatitude) { SRID = 4326 };
    }

    public static List<LogisticCenter> GenerateLogisticCenters(int count, double latitude, double longitude, double maxDistanceKm)
    {
        var centers = new List<LogisticCenter>();
        for (int i = 0; i < count; i++)
        {
            centers.Add(new LogisticCenter
            {
                Id = i + 1,
                Name = $"Logistic Center {i + 1}",
                Location = GenerateRandomPoint(latitude, longitude, maxDistanceKm)
            });
        }
        return centers;
    }
}