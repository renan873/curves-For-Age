using System.Windows.Input;
using CurvesForAge.Models;
using CurvesForAge.Views;

namespace CurvesForAge.ViewModels;

public class MainViewModel : ViewModelBase
{
    private DateTime _maxDate = DateTime.Today;
    private DateTime _selectedDate = DateTime.Today;
    private DateTime _takenDate = DateTime.Today;
    private float _height;
    private float _weight;
    private bool _headCircumferenceVisible = true;
    private float _headCircumference;
    private string? _sex;

    #region bindings

    public DateTime MaxDate
    {
        get => _maxDate;
        set => SetValue(ref _maxDate, value);
    }

    public DateTime SelectedDate
    {
        get => _selectedDate;
        set => SetValue(ref _selectedDate, value, ChangedSelectedDate);
    }

    public DateTime TakenDate
    {
        get => _takenDate;
        set => SetValue(ref _takenDate, value, ChangedSelectedDate);
    }

    public float Height
    {
        get => _height;
        set => SetValue(ref _height, value, ChangedHeight);
    }

    public float Weight
    {
        get => _weight;
        set => SetValue(ref _weight, value, ChangedWeight);
    }

    public bool HeadCircumferenceVisible
    {
        get => _headCircumferenceVisible;
        set => SetValue(ref _headCircumferenceVisible, value);
    }

    public float HeadCircumference
    {
        get => _headCircumference;
        set => SetValue(ref _headCircumference, value, ChangedHeadCircumference);
    }

    public string? Sex
    {
        get => _sex;
        set => SetValue(ref _sex, value);
    }

    public ICommand CalcCommand { get; private set; }
    public INavigation? Navigation { get; set; }

    #endregion

    public MainViewModel()
    {
        CalcCommand = new Command(CalcMethod);
    }

    private async void CalcMethod()
    {
        var bmi = float.Round(Weight * 10000 / (Height * Height), 2);

        if (string.IsNullOrEmpty(Sex))
        {
            Application.Current?
                .MainPage?.DisplayAlert("Alerta", "El Sexo de la persona es requerido", "Ok");
            return;
        }


        if (Height < 10)
        {
            Application.Current?
                .MainPage?.DisplayAlert("Alerta", "La estatura debe ser superior a 10 cm", "Ok");
            return;
        }


        if (Weight < 1)
        {
            Application.Current?
                .MainPage?.DisplayAlert("Alerta", "El peso no puede ser inferior a 1 Kg", "Ok");
            return;
        }

        if (bmi > 50)
        {
            Application.Current?
                .MainPage?.DisplayAlert("Alerta", $"El IMC calculado es de {bmi} " +
                                                  $"por lo que sale completamente del cálculo (IMC máximo de 50)" +
                                                  $"", "Ok");
            return;
        }

        if (TakenDate < SelectedDate)
        {
            Application.Current?
                .MainPage?.DisplayAlert("Alerta", "La fecha de toma de datos no puede ser inferior a " +
                                                  "la fecha de nacimiento", "Ok");
            return;
        }
        
        if((TakenDate - SelectedDate).TotalDays >= 6980)
        {
            Application.Current?
                .MainPage?.DisplayAlert("Alerta", "Por la edad de la persona no se va a generar un gráfico, " + 
                                                  "para poder hacer el calculo se requiere una persona menor de 19 años", 
                    "Ok");
        }

        await Navigation?.PushAsync(new ResultPage(new DataForAgesRequest
        {
            Sex = Sex,
            Height = Height,
            Weight = Weight,
            Hc = HeadCircumference,
            Dob = SelectedDate,
            Take = TakenDate
        }))!;
    }

    private void ChangedSelectedDate() => HeadCircumferenceVisible = SelectedDate > DateTime.Today.AddYears(-2);

    private void ChangedHeight() => Height = Height > 250 ? 250 : Height;

    private void ChangedWeight() => Weight = Weight > 300 ? 300 : Weight;

    private void ChangedHeadCircumference() => HeadCircumference = HeadCircumference > 90 ? 90 : HeadCircumference;
}