using System.Reflection;
using CurvesForAge.ViewModels;

namespace CurvesForAge;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        var viewModel = new MainViewModel();
        viewModel.Navigation = Navigation;
        BindingContext = viewModel;
    }
}