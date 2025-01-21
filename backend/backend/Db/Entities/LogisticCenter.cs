using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace backend.Db.Entities;

[Table("logistic_centers")]
public class LogisticCenter
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column(name:"location", TypeName="geography")]
    public Point Location { get; set; }
}