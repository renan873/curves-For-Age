using CurvesForAge.ViewModels;

namespace CurvesForAge;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        var viewModel = new MainPageViewModel();
        BindingContext = viewModel;
    }
}