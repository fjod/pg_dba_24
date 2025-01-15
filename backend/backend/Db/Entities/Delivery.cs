using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace backend.Db.Entities;

[Table("deliveries")]
public class Delivery
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("order_id")]
    public int OrderId { get; set; }
    
    [Column(name:"point", TypeName="geography")]
    public Point Point { get; set; }
    
    [Column("delivery_timestamp")]
    public DateTime DeliveryTimestamp { get; set; }
    
    [Column("year")]
    public int Year { get; set; }
    public Order Order { get; set; }
}