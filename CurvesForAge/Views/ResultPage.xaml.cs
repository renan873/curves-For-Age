using CurvesForAge.ViewModels;

namespace CurvesForAge.Views;

public partial class ResultPage : ContentPage
{
    public ResultPage()
    {
        InitializeComponent();
        var viewModel = new ResultPageViewModel();
        BindingContext = viewModel;
    }
}