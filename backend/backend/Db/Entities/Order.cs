using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Db.Entities;

[Table("orders")]
public class Order
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("courier_id")]
    public int CourierId { get; set; }
    
    [Column("order_timestamp")]
    public DateTime OrderTimestamp { get; set; }
    public Courier Courier { get; set; }
}