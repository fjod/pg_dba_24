using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace backend.Db.Entities;

public class Delivery
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    [Column(TypeName="geography")]
    public Point Point { get; set; }
    public DateTime DeliveryTimestamp { get; set; }
    public int YearMonth { get; set; }
    public Order Order { get; set; }
}