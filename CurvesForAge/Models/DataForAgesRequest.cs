namespace CurvesForAge.Models;

public class DataForAgesRequest
{
    public string Sex { get; set; }
    public float Height { get; set; }
    public float Weight { get; set; }
    public float Hc { get; set; }
    public DateTime Dob { get; set; }
}