using System.Web;

namespace CurvesForAge.ViewModels;

public class ResultPageViewModel : ViewModelBase, IQueryAttributable
{
    #region bindings

    public string? Sex { get; private set; }
    public float Height { get; private set; }
    public float Weight { get; private set; }
    public float Hc { get; private set; }
    public DateTime Dob { get; private set; }

    #endregion

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Sex = HttpUtility.UrlDecode(query["sex"].ToString());
        var height = HttpUtility.UrlDecode(query["height"].ToString());
        var weight = HttpUtility.UrlDecode(query["weight"].ToString());
        var hc = HttpUtility.UrlDecode(query["hc"].ToString());
        var dob = HttpUtility.UrlDecode(query["dob"].ToString());

        float.TryParse(height, out var result);
        Height = result;
        
        float.TryParse(weight, out result);
        Weight = result;
        
        float.TryParse(hc, out result);
        Hc = result;

        DateTime.TryParse(dob, out DateTime dateTime);
        Dob = dateTime;
    }
}