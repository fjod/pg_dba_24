public class DateTimeGenerator
{
    private static readonly Random Random = new Random();

    public DateTime GenerateRandomDateTime()
    {
        DateTime start = new DateTime(2020, 1, 1);
        DateTime end = new DateTime(2025, 12, 31);
        int range = (end - start).Days;
        return start.AddDays(Random.Next(range)).AddSeconds(Random.Next(86400)); // Add random seconds within a day
    }
}