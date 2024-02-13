using SQLite;

namespace CurvesForAge.Models;

[Table("DataForAges")]
public class DataForAge
{
    [PrimaryKey, AutoIncrement, Column("id")]
    public int Id { get; set; }
    
    [Column("type")]
    public string? Type { get; set; }
    
    [Column("sex")]
    public string? Sex { get; set; }
    
    [Column("days")]
    public int Days { get; set; }
    
    [Column("sd4neg")]
    public decimal Sd4Neg { get; set; }
    
    [Column("sd3neg")]
    public decimal Sd3Neg { get; set; }
    
    [Column("sd2neg")]
    public decimal Sd2Neg { get; set; }
    
    [Column("sd1neg")]
    public decimal Sd1Neg { get; set; }
    
    [Column("sd0")]
    public decimal Sd0 { get; set; }
    
    [Column("sd1")]
    public decimal Sd1 { get; set; }
    
    [Column("sd2")]
    public decimal Sd2 { get; set; }
    
    [Column("sd3")]
    public decimal Sd3 { get; set; }
    
    [Column("sd4")]
    public decimal Sd4 { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}