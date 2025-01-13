namespace backend.Db.Entities;

public class Courier
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int LogisticCenterId { get; set; }
    public LogisticCenter LogisticCenter { get; set; }
}