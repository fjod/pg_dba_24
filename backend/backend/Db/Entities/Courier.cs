using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Db.Entities;

[Table("couriers")]
public class Courier
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("logistic_center_id")]
    public int LogisticCenterId { get; set; }
    public LogisticCenter LogisticCenter { get; set; }
}