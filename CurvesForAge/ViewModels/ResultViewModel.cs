using CurvesForAge.Data;
using CurvesForAge.Models;

namespace CurvesForAge.ViewModels;

public class ResultViewModel : ViewModelBase
{
    private readonly DataForAgesRequest _request;
    private readonly DatabaseContext _context;

    private string _mainMessage = "";
    private string _heightResult = "";
    private string _weightResult = "";
    private string _bmiResult = "";
    private string _hcResult = "";

    public string MainMessage
    {
        get => _mainMessage;
        set => SetValue(ref _mainMessage, value);
    }

    public string HeightResult
    {
        get => _heightResult;
        set => SetValue(ref _heightResult, value);
    }

    public string WeightResult
    {
        get => _weightResult;
        set => SetValue(ref _weightResult, value);
    }

    public string BmiResult
    {
        get => _bmiResult;
        set => SetValue(ref _bmiResult, value);
    }

    public string HcResult
    {
        get => _hcResult;
        set => SetValue(ref _hcResult, value);
    }

    public ResultViewModel(DataForAgesRequest request)
    {
        _request = request;
        _context = new DatabaseContext();
    }

    public async Task LoadDataAsync()
    {
        var sex = _request.Sex;

        var days = (DateTime.Now - _request.Dob).TotalDays;

        var dataForAgeTable = await _context.GetTableAsync<DataForAge>();

        MainMessage = $"De acuerdo a los datos ingresados se calcula el IMC por un valor de {_request.Bmi}, " +
                      $"lo que nos da el siguiente resultado:";

        var heightResult = await dataForAgeTable
            .Where(x => x.Sex == sex && x.Type == "HeightForAge" && x.Days < days)
            .OrderByDescending(x => x.Days)
            .FirstAsync() ?? new DataForAge();

        var weightResult = await dataForAgeTable
            .Where(x => x.Sex == sex && x.Type == "WeightForAge" && x.Days < days)
            .OrderByDescending(x => x.Days)
            .FirstAsync() ?? new DataForAge();

        var bmiResult = await dataForAgeTable
            .Where(x => x.Sex == sex && x.Type == "BMIForAge" && x.Days < days)
            .OrderByDescending(x => x.Days)
            .FirstAsync() ?? new DataForAge();

        HeightResult = DefineCase(heightResult, _request.Height, "una talla - longitud") + ".";
        WeightResult = DefineCase(weightResult, _request.Weight, "un peso") + ".";
        BmiResult = DefineCase(bmiResult, _request.Bmi, "un IMC") + ".";

        if (days < 730)
        {
            var hcResult = await dataForAgeTable
                .Where(x => x.Sex == sex && x.Type == "HCForAge" && x.Days < days)
                .OrderByDescending(x => x.Days)
                .FirstAsync() ?? new DataForAge();
            
            HcResult = DefineCase(hcResult, _request.Hc, "un perímetro cefálico") + ".";
        }

        //Application.Current?.MainPage?.DisplayAlert("Alerta",
        //    $"el registro es de {bmiResult.Days} días para del sexo {sex}", "Ok");
    }

    private string DefineCase(DataForAge register, float value, string text)
    {
        var isMale = text.Substring(0, 3) != "una";

        string low = isMale ? "bajo" : "baja";
        string high = isMale ? "alto" : "alta";

        if ((float) register.Sd3Neg > value)
        {
            return $"La persona tiene {text} por debajo de la DE -3, " +
                   $" lo que indica que tiene {text} muy {low} para la edad";
        }

        if ((float) register.Sd2Neg > value)
        {
            return $"La persona tiene {text} entre la DE -2 y la DE -3, " +
                   $" lo que indica que tiene {text} {low} para la edad";
        }

        if ((float) register.Sd1Neg > value)
        {
            return $"La persona tiene {text} entre la DE -1 y la DE -2, " +
                   $" lo que indica que tiene {text} normal para la edad";
        }

        if ((float) register.Sd0 > value)
        {
            return $"La persona tiene {text} entre la DE 0 y la DE -1, " +
                   $" lo que indica que tiene {text} normal para la edad";
        }

        if ((float) register.Sd1 > value)
        {
            return $"La persona tiene {text} entre la DE 1 y la DE 0, " +
                   $" lo que indica que tiene {text} normal para la edad";
        }

        if ((float) register.Sd2 > value)
        {
            return $"La persona tiene {text} entre la DE 2 y la DE 1, " +
                   $" lo que indica que tiene {text} normal para la edad";
        }

        if ((float) register.Sd3 > value)
        {
            return $"La persona tiene {text} entre la DE 3 y la DE 2, " +
                   $" lo que indica que tiene {text} {high} para la edad";
        }

        return $"La persona tiene {text} por encima de la DE 3, " +
               $" lo que indica que tiene {text} muy {high} para la edad";
    }
}