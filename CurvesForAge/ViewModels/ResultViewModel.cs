using CurvesForAge.Data;
using CurvesForAge.Models;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace CurvesForAge.ViewModels;

public class ResultViewModel(DataForAgesRequest request) : ViewModelBase
{
    private readonly DatabaseContext _context = new();

    private string _mainMessage = "";
    private string _bmiResult = "";
    private string _weightResult = "";
    private string _heightResult = "";
    private string _hcResult = "";

    private bool _bmiResultVisible = false;
    private bool _weightResultVisible = false;
    private bool _heightResultVisible = false;
    private bool _hcResultVisible = false;

    private ISeries[] _bmiSeries = Array.Empty<ISeries>();
    private ISeries[] _weightSeries = Array.Empty<ISeries>();
    private ISeries[] _heightSeries = Array.Empty<ISeries>();
    private ISeries[] _hcSeries = Array.Empty<ISeries>();

    #region Bindings

    public string MainMessage
    {
        get => _mainMessage;
        set => SetValue(ref _mainMessage, value);
    }

    public string BmiResult
    {
        get => _bmiResult;
        set => SetValue(ref _bmiResult, value);
    }

    public string WeightResult
    {
        get => _weightResult;
        set => SetValue(ref _weightResult, value);
    }

    public string HeightResult
    {
        get => _heightResult;
        set => SetValue(ref _heightResult, value);
    }

    public bool BmiResultVisible
    {
        get => _bmiResultVisible;
        set => SetValue(ref _bmiResultVisible, value);
    }

    public bool WeightResultVisible
    {
        get => _weightResultVisible;
        set => SetValue(ref _weightResultVisible, value);
    }

    public bool HeightResultVisible
    {
        get => _heightResultVisible;
        set => SetValue(ref _heightResultVisible, value);
    }
    
    public bool HcResultVisible
    {
        get => _hcResultVisible;
        set => SetValue(ref _hcResultVisible, value);
    }

    public string HcResult
    {
        get => _hcResult;
        set => SetValue(ref _hcResult, value);
    }

    public ISeries[] HeightSeries
    {
        get => _heightSeries;
        set => SetValue(ref _heightSeries, value);
    }

    public ISeries[] WeightSeries
    {
        get => _weightSeries;
        set => SetValue(ref _weightSeries, value);
    }

    public ISeries[] BmiSeries
    {
        get => _bmiSeries;
        set => SetValue(ref _bmiSeries, value);
    }

    public ISeries[] HcSeries
    {
        get => _hcSeries;
        set => SetValue(ref _hcSeries, value);
    }

    #endregion


    public async Task LoadDataAsync()
    {
        var sex = request.Sex;

        var days = (DateTime.Now - request.Dob).TotalDays;

        var dataForAgeTable = await _context.GetTableAsync<DataForAge>();

        MainMessage = $"De acuerdo a los datos ingresados se calcula el IMC por un valor de {request.Bmi}, " +
                      $"lo que nos da el siguiente resultado:";

        var bmiResult = await dataForAgeTable
            .Where(x => x.Sex == sex && x.Type == "BMIForAge" && x.Days < days)
            .OrderByDescending(x => x.Days)
            .FirstAsync() ?? new DataForAge();

        var heightResult = await dataForAgeTable
            .Where(x => x.Sex == sex && x.Type == "HeightForAge" && x.Days < days)
            .OrderByDescending(x => x.Days)
            .FirstAsync() ?? new DataForAge();

        BmiResult = DefineCase(bmiResult, request.Bmi, "un IMC") + ".";
        HeightResult = DefineCase(heightResult, request.Height, "una talla - longitud") + ".";

        if (days < 6980)
        {
            BmiSeries = await ChartGeneration("BMIForAge", new ObservablePoint(days, request.Bmi));
            BmiResultVisible = true;
            HeightSeries = await ChartGeneration("HeightForAge", new ObservablePoint(days, request.Height));
            HeightResultVisible = true;
        }

        if (days < 3686)
        {
            var weightResult = await dataForAgeTable
                .Where(x => x.Sex == sex && x.Type == "WeightForAge" && x.Days < days)
                .OrderByDescending(x => x.Days)
                .FirstAsync() ?? new DataForAge();

            WeightResult = DefineCase(weightResult, request.Weight, "un peso") + ".";
            WeightSeries = await ChartGeneration("WeightForAge", new ObservablePoint(days, request.Weight));
            WeightResultVisible = true;
        }

        if (days < 1856)
        {
            var hcResult = await dataForAgeTable
                .Where(x => x.Sex == sex && x.Type == "HCForAge" && x.Days < days)
                .OrderByDescending(x => x.Days)
                .FirstAsync() ?? new DataForAge();

            HcResult = DefineCase(hcResult, request.Hc, "un perímetro cefálico") + ".";
            HcSeries = await ChartGeneration("HCForAge", new ObservablePoint(days, request.Hc));
            HcResultVisible = true;
        }

        //Application.Current?.MainPage?.DisplayAlert("Alerta",
        //    $"el registro es de {bmiResult.Days} días para del sexo {sex}", "Ok");
    }

    private async Task<LineSeries<ObservablePoint>[]> ChartGeneration(string type, ObservablePoint observablePoint)
    {
        var dataForAgeTable = await _context.GetTableAsync<DataForAge>();

        var initDay = observablePoint.X - 500;
        var lastDay = observablePoint.X + 500;

        var data = await dataForAgeTable
            .Where(x => x.Sex == request.Sex && x.Type == type && x.Days > initDay && x.Days < lastDay)
            .OrderBy(x => x.Days)
            .ToListAsync() ?? new List<DataForAge>();

        List<ObservablePoint> dMinus4Array = new(),
            dMinus3Array = new(),
            dMinus2Array = new(),
            dMinus1Array = new(),
            d0Array = new(),
            d1Array = new(),
            d2Array = new(),
            d3Array = new(),
            d4Array = new();

        if (data.Any())
        {
            foreach (var record in data)
            {
                dMinus4Array.Add(new ObservablePoint(record.Days, (double) record.Sd4Neg));
                dMinus3Array.Add(new ObservablePoint(record.Days, (double) record.Sd3Neg));
                dMinus2Array.Add(new ObservablePoint(record.Days, (double) record.Sd2Neg));
                dMinus1Array.Add(new ObservablePoint(record.Days, (double) record.Sd1Neg));
                d0Array.Add(new ObservablePoint(record.Days, (double) record.Sd0));
                d1Array.Add(new ObservablePoint(record.Days, (double) record.Sd1));
                d2Array.Add(new ObservablePoint(record.Days, (double) record.Sd2));
                d3Array.Add(new ObservablePoint(record.Days, (double) record.Sd3));
                d4Array.Add(new ObservablePoint(record.Days, (double) record.Sd4));
            }
        }

        LineSeries<ObservablePoint> dMinus4 = new()
            {
                Values = dMinus4Array.ToArray(),
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 1,
                Stroke = new SolidColorPaint(SKColors.Red),
                GeometryStroke = new SolidColorPaint(SKColors.Red)
            },
            dMinus3 = new()
            {
                Values = dMinus3Array.ToArray(),
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 1,
                Stroke = new SolidColorPaint(SKColors.Orange),
                GeometryStroke = new SolidColorPaint(SKColors.Orange)
            },
            dMinus2 = new()
            {
                Values = dMinus2Array.ToArray(),
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 1,
                Stroke = new SolidColorPaint(SKColors.Yellow),
                GeometryStroke = new SolidColorPaint(SKColors.Yellow)
            },
            dMinus1 = new()
            {
                Values = dMinus1Array.ToArray(),
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 1,
                Stroke = new SolidColorPaint(SKColors.GreenYellow),
                GeometryStroke = new SolidColorPaint(SKColors.GreenYellow)
            },
            d0 = new()
            {
                Values = d0Array.ToArray(),
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 1,
                Stroke = new SolidColorPaint(SKColors.Green),
                GeometryStroke = new SolidColorPaint(SKColors.Green)
            },
            d1 = new()
            {
                Values = d1Array.ToArray(),
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 1,
                Stroke = new SolidColorPaint(SKColors.GreenYellow),
                GeometryStroke = new SolidColorPaint(SKColors.GreenYellow)
            },
            d2 = new()
            {
                Values = d2Array.ToArray(),
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 1,
                Stroke = new SolidColorPaint(SKColors.Yellow),
                GeometryStroke = new SolidColorPaint(SKColors.Yellow)
            },
            d3 = new()
            {
                Values = d3Array.ToArray(),
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 1,
                Stroke = new SolidColorPaint(SKColors.Orange),
                GeometryStroke = new SolidColorPaint(SKColors.Orange)
            },
            d4 = new()
            {
                Values = d4Array.ToArray(),
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 1,
                Stroke = new SolidColorPaint(SKColors.Red),
                GeometryStroke = new SolidColorPaint(SKColors.Red)
            },
            result = new()
            {
                Values = new[] {observablePoint},
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 1,
                Stroke = new SolidColorPaint(SKColors.Black) {StrokeThickness = 6},
                GeometryStroke = new SolidColorPaint(SKColors.Black) {StrokeThickness = 6},
            };

        return
        [
            dMinus4,
            dMinus3,
            dMinus2,
            dMinus1,
            d0,
            d1,
            d2,
            d3,
            d4,
            result
        ];
    }

    private string DefineCase(DataForAge register, float value, string text)
    {
        var isMale = text.Substring(0, 3) != "una";

        string low = isMale ? "bajo" : "baja";
        string high = isMale ? "alto" : "alta";

        if ((float) register.Sd4Neg > value)
        {
            return $"La persona tiene {text} por debajo de la DE -4, " +
                   $" lo que indica que tiene {text} muy {low} para la edad";
        }

        if ((float) register.Sd3Neg > value)
        {
            return $"La persona tiene {text} entre la DE -3 y la DE -4, " +
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

        if ((float) register.Sd4 > value)
        {
            return $"La persona tiene {text} entre la DE 4 y la DE 3, " +
                   $" lo que indica que tiene {text} {high} para la edad";
        }

        return $"La persona tiene {text} por encima de la DE 4, " +
               $" lo que indica que tiene {text} muy {high} para la edad";
    }
}