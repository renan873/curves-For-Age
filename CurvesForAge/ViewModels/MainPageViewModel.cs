using System.Windows.Input;

namespace CurvesForAge.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    private DateTime _maxDate = DateTime.Today;
    private DateTime _selectedDate = DateTime.Today;
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
        set => SetValue(ref _sex, value, ChangesSex);
    }

    private void ChangesSex()
    {
        Application.Current?.MainPage?.DisplayAlert("Sexo", Sex, "Ok");
    }
    
    public ICommand CalcCommand { get; private set; }

    #endregion

    public MainPageViewModel()
    {
        CalcCommand = new Command(CalcMethod);
    }

    private async void CalcMethod()
    {
        if (string.IsNullOrEmpty(Sex))
        {
            Application.Current?.MainPage?.DisplayAlert("Alerta","El Sexo de la persona es requerido", "Ok");
            return;
        }
        
        await Shell.Current.GoToAsync($"///ResultPage?height={Height}&weight={Weight}&hc={HeadCircumference}&dob={SelectedDate}&sex={Sex}");
    }

    private void ChangedSelectedDate() => HeadCircumferenceVisible = SelectedDate > DateTime.Today.AddYears(-2);

    private void ChangedHeight() => Height = Height > 250 ? 250 : Height;

    private void ChangedWeight() => Weight = Weight > 250 ? 250 : Weight;
    
    private void ChangedHeadCircumference() => HeadCircumference = HeadCircumference > 90 ? 90 : HeadCircumference;
}