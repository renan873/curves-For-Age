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
    private string _bmiText = "";
    private string _heightText = "";
    private string _weightTextInit = ".";
    private string _weightText = "";
    private string _bmiResult = "";
    private string _weightResult = "";
    private string _heightResult = "";
    private string _hcResult = "";

    private bool _resultVisible;
    private bool _bmiResultVisible;
    private bool _weightResultVisible;
    private bool _heightResultVisible;
    private bool _hcResultVisible;

    private ISeries[] _bmiSeries = Array.Empty<ISeries>();
    private ISeries[] _weightSeries = Array.Empty<ISeries>();
    private ISeries[] _heightSeries = Array.Empty<ISeries>();
    private ISeries[] _hcSeries = Array.Empty<ISeries>();
    private Axis[] _xAxeBmi = [new TimeSpanAxis(TimeSpan.FromDays(365), timeSpan => timeSpan.Days / 365 + " años")];
    private Axis[] _xAxeHeight = [new TimeSpanAxis(TimeSpan.FromDays(365), timeSpan => timeSpan.Days / 365 + " años")];
    private Axis[] _xAxeWeight = [new TimeSpanAxis(TimeSpan.FromDays(365), timeSpan => timeSpan.Days / 365 + " años")];
    private Axis[] _xAxeHc = [new TimeSpanAxis(TimeSpan.FromDays(365), timeSpan => timeSpan.Days / 365 + " años")];

    #region Bindings

    public string MainMessage
    {
        get => _mainMessage;
        set => SetValue(ref _mainMessage, value);
    }

    public string BmiText
    {
        get => _bmiText;
        set => SetValue(ref _bmiText, value);
    }

    public string HeightText
    {
        get => _heightText;
        set => SetValue(ref _heightText, value);
    }

    public string WeightTextInit
    {
        get => _weightTextInit;
        set => SetValue(ref _weightTextInit, value);
    }

    public string WeightText
    {
        get => _weightText;
        set => SetValue(ref _weightText, value);
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

    public bool ResultVisible
    {
        get => _resultVisible;
        set => SetValue(ref _resultVisible, value);
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

    public Axis[] XAxeBmi
    {
        get => _xAxeBmi;
        set => SetValue(ref _xAxeBmi, value);
    }

    public Axis[] XAxeHeight
    {
        get => _xAxeHeight;
        set => SetValue(ref _xAxeHeight, value);
    }

    public Axis[] XAxeWeight
    {
        get => _xAxeWeight;
        set => SetValue(ref _xAxeWeight, value);
    }

    public Axis[] XAxeHc
    {
        get => _xAxeHc;
        set => SetValue(ref _xAxeHc, value);
    }

    #endregion


    public async Task LoadDataAsync()
    {
        try
        {
            var sex = request.Sex;

            var days = (request.Take - request.Dob).TotalDays;

            var dataForAgeTable = await _context.GetTableAsync<DataForAge>();

            int? weightCase = null;

            MainMessage = $"De acuerdo a los datos ingresados se calcula el IMC por un valor de {request.Bmi}, " +
                          $"lo que nos da el siguiente resultado:";

            var bmiResult = await dataForAgeTable
                .Where(x => x.Sex == sex && x.Type == "BMIForAge" && x.Days <= days)
                .OrderByDescending(x => x.Days)
                .FirstAsync() ?? new DataForAge();

            var heightResult = await dataForAgeTable
                .Where(x => x.Sex == sex && x.Type == "HeightForAge" && x.Days <= days)
                .OrderByDescending(x => x.Days)
                .FirstAsync() ?? new DataForAge();

            var bmiResultTuple = DefineCase(bmiResult, request.Bmi, "un IMC");
            var heightResultTuple = DefineCase(heightResult, request.Height, "una talla - longitud");

            BmiResult = bmiResultTuple.Item1 + ".";
            HeightResult = heightResultTuple.Item1 + ".";

            if (days < 6980)
            {
                BmiSeries = await ChartGeneration("BMIForAge", new TimeSpanPoint(TimeSpan.FromDays(days), request.Bmi));
                BmiResultVisible = true;
                HeightSeries = await ChartGeneration("HeightForAge",
                    new TimeSpanPoint(TimeSpan.FromDays(days), request.Height));
                HeightResultVisible = true;
            }

            if (days < 3686)
            {
                var weightResult = await dataForAgeTable
                    .Where(x => x.Sex == sex && x.Type == "WeightForAge" && x.Days <= days)
                    .OrderByDescending(x => x.Days)
                    .FirstAsync() ?? new DataForAge();

                var weightResultTuple = DefineCase(weightResult, request.Weight, "un peso");
                WeightResult = weightResultTuple.Item1 + ".";
                WeightSeries = await ChartGeneration("WeightForAge",
                    new TimeSpanPoint(TimeSpan.FromDays(days), request.Weight));
                WeightResultVisible = true;

                weightCase = weightResultTuple.Item2;
            }

            if (days < 1856 || request.Hc == 0)
            {
                var hcResult = await dataForAgeTable
                    .Where(x => x.Sex == sex && x.Type == "HCForAge" && x.Days <= days)
                    .OrderByDescending(x => x.Days)
                    .FirstAsync() ?? new DataForAge();

                var hcResultTuple = DefineCase(hcResult, request.Hc, "un perímetro cefálico");
                HcResult = hcResultTuple.Item1 + ".";
                HcSeries = await ChartGeneration("HCForAge", new TimeSpanPoint(TimeSpan.FromDays(days), request.Hc));
                HcResultVisible = true;
            }

            AnalysisResult(bmiResultTuple.Item2, heightResultTuple.Item2, weightCase);
        }
        catch (Exception e)
        {
            Application.Current?.MainPage?.DisplayAlert("Error",
                e.Message, "Ok");
        }
    }

    private async Task<LineSeries<TimeSpanPoint>[]> ChartGeneration(string type, TimeSpanPoint observablePoint)
    {
        var dataForAgeTable = await _context.GetTableAsync<DataForAge>();

        var initDay = observablePoint.TimeSpan.Days - 1000;
        var lastDay = observablePoint.TimeSpan.Days + 1000;

        var data = await dataForAgeTable
            .Where(x => x.Sex == request.Sex && x.Type == type && x.Days > initDay && x.Days < lastDay)
            .OrderBy(x => x.Days)
            .ToListAsync() ?? [];

        List<TimeSpanPoint> dMinus4Array = [],
            dMinus3Array = [],
            dMinus2Array = [],
            dMinus1Array = [],
            d0Array = [],
            d1Array = [],
            d2Array = [],
            d3Array = [],
            d4Array = [];

        if (data is {Count: > 0})
        {
            foreach (var record in data)
            {
                dMinus4Array.Add(new TimeSpanPoint(TimeSpan.FromDays(record.Days), (double) record.Sd4Neg));
                dMinus3Array.Add(new TimeSpanPoint(TimeSpan.FromDays(record.Days), (double) record.Sd3Neg));
                dMinus2Array.Add(new TimeSpanPoint(TimeSpan.FromDays(record.Days), (double) record.Sd2Neg));
                dMinus1Array.Add(new TimeSpanPoint(TimeSpan.FromDays(record.Days), (double) record.Sd1Neg));
                d0Array.Add(new TimeSpanPoint(TimeSpan.FromDays(record.Days), (double) record.Sd0));
                d1Array.Add(new TimeSpanPoint(TimeSpan.FromDays(record.Days), (double) record.Sd1));
                d2Array.Add(new TimeSpanPoint(TimeSpan.FromDays(record.Days), (double) record.Sd2));
                d3Array.Add(new TimeSpanPoint(TimeSpan.FromDays(record.Days), (double) record.Sd3));
                d4Array.Add(new TimeSpanPoint(TimeSpan.FromDays(record.Days), (double) record.Sd4));
            }
        }

        LineSeries<TimeSpanPoint> dMinus4 = new()
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
                GeometrySize = 2,
                GeometryFill = new SolidColorPaint(SKColors.Black),
                LineSmoothness = 1,
                Stroke = new SolidColorPaint(SKColors.Black) {StrokeThickness = 6},
                GeometryStroke = new SolidColorPaint(SKColors.Black) {StrokeThickness = 3}
            };

        return
        [
            //dMinus4,
            dMinus3,
            dMinus2,
            //dMinus1,
            d0,
            //d1,
            d2,
            d3,
            //d4,
            result
        ];
    }

    private Tuple<string, int> DefineCase(DataForAge register, float value, string text)
    {
        var isMale = text[..3] != "una";

        var low = isMale ? "bajo" : "baja";
        var high = isMale ? "alto" : "alta";

        if ((float) register.Sd4Neg > value)
        {
            return new Tuple<string, int>(
                $"La persona tiene {text} por debajo de la DE -4, lo que indica que tiene {text} muy {low} para la edad",
                -4
            );
        }

        if ((float) register.Sd3Neg > value)
        {
            return new Tuple<string, int>(
                $"La persona tiene {text} entre la DE -3 y la DE -4, lo que indica que tiene {text} muy {low} para la edad",
                -3
            );
        }

        if ((float) register.Sd2Neg > value)
        {
            return new Tuple<string, int>(
                $"La persona tiene {text} entre la DE -2 y la DE -3, lo que indica que tiene {text} {low} para la edad",
                -2
            );
        }

        if ((float) register.Sd1Neg > value)
        {
            return new Tuple<string, int>(
                $"La persona tiene {text} entre la DE -1 y la DE -2, lo que indica que tiene {text} normal para la edad",
                -1
            );
        }

        if ((float) register.Sd0 > value)
        {
            return new Tuple<string, int>(
                $"La persona tiene {text} entre la DE 0 y la DE -1, lo que indica que tiene {text} normal para la edad",
                0
            );
        }

        if ((float) register.Sd1 > value)
        {
            return new Tuple<string, int>(
                $"La persona tiene {text} entre la DE 1 y la DE 0, lo que indica que tiene {text} normal para la edad",
                0
            );
        }

        if ((float) register.Sd2 > value)
        {
            return new Tuple<string, int>(
                $"La persona tiene {text} entre la DE 2 y la DE 1, lo que indica que tiene {text} normal para la edad",
                1
            );
        }

        if ((float) register.Sd3 > value)
        {
            return new Tuple<string, int>(
                $"La persona tiene {text} entre la DE 3 y la DE 2, lo que indica que tiene {text} {high} para la edad",
                2
            );
        }

        if ((float) register.Sd4 > value)
        {
            return new Tuple<string, int>(
                $"La persona tiene {text} entre la DE 4 y la DE 3, lo que indica que tiene {text} {high} para la edad",
                3
            );
        }

        return new Tuple<string, int>(
            $"La persona tiene {text} por encima de la DE 4, lo que indica que tiene {text} muy {high} para la edad",
            4
        );
    }

    private void AnalysisResult(int bmiCase, int heightCase, int? weightCase)
    {
        BmiText = bmiCase switch
        {
            > 2 => "obeso",
            2 => "con sobrepeso",
            1 => "con riesgo de sobrepeso",
            -3 or -2 => "emaciado",
            < -3 => "severamente emaciado",
            _ => "normal"
        };

        HeightText = heightCase switch
        {
            -2 => "baja talla",
            < -2 => "baja talla severa",
            _ => "normal"
        };

        if (weightCase != null)
        {
            WeightTextInit = ", en función del peso se tiene que el estado es ";

            WeightText += weightCase switch
            {
                -2 => "bajo peso",
                < -2 => "bajo peso severo",
                _ => "normal"
            } + ".";
        }
        ResultVisible = true;
    }
}