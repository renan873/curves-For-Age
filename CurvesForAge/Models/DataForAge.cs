using SQLite;

namespace CurvesForAge.Models;

[Table("DataForAges")]
public class DataForAge
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public string type { get; set; }
    public string sex { get; set; }
    public int days { get; set; }
    public decimal sd4neg { get; set; }
    public decimal sd3neg { get; set; }
    public decimal sd2neg { get; set; }
    public decimal sd1neg { get; set; }
    public decimal sd0 { get; set; }
    public decimal sd1 { get; set; }
    public decimal sd2 { get; set; }
    public decimal sd3 { get; set; }
    public decimal sd4 { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}