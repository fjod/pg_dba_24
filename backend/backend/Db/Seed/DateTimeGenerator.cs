namespace backend.Db.Seed;

public class DateTimeGenerator
{
    private static readonly Random Random = new Random();

    private static readonly DateTime Start = new DateTime(2020, 1, 1);
    private static int DaysRange => (new DateTime(2025, 11, 20) - new DateTime(2020, 1, 1)).Days;
    public DateTime GenerateRandomDateTime()
    {
        return Start.AddDays(Random.Next(DaysRange));
    }
}