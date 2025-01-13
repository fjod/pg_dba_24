using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace backend.Db.Entities;

public class LogisticCenter
{
    public int Id { get; set; }
    public string Name { get; set; }
    [Column(TypeName="geography")]
    public Point Location { get; set; }
}