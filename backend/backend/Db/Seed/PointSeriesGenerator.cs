using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

public static class PointSeriesGenerator
{
    private static readonly Random Random = new Random();

    public static List<Point> GeneratePointSeries(Point start, int minPoints, int maxPoints, double minLength, double maxLength)
    {
        int pointCount = Random.Next(minPoints, maxPoints + 1);
        double totalLength = minLength + (maxLength - minLength) * Random.NextDouble();
        double segmentLength = totalLength / pointCount;

        var points = new List<Point> { start };
        Point currentPoint = start;

        for (int i = 1; i < pointCount; i++)
        {
            double angle = 2 * Math.PI * Random.NextDouble();
            double dx = segmentLength * Math.Cos(angle);
            double dy = segmentLength * Math.Sin(angle);

            double newLatitude = currentPoint.Y + dy / 111.32;
            double newLongitude = currentPoint.X + dx / (111.32 * Math.Cos(currentPoint.Y * Math.PI / 180));

            currentPoint = new Point(newLongitude, newLatitude) { SRID = 4326 };
            points.Add(currentPoint);
        }

        return points;
    }
}