namespace backend.Db.Entities;

public class Order
{
    public int Id { get; set; }
    public int CourierId { get; set; }
    public DateTime OrderTimestamp { get; set; }
    public Courier Courier { get; set; }
}