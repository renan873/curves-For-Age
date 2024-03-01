namespace CurvesForAge.Models;

public class DataForAgesRequest
{
    public string? Sex { get; set; }
    public float Height { get; set; }
    public float Weight { get; set; }
    public float Hc { get; set; }
    public float Bmi => float.Round(Weight * 10000 / (Height * Height), 2);
    public DateTime Dob { get; set; }
    public DateTime Take { get; set; }
}