using CurvesForAge.Models;
using CurvesForAge.ViewModels;

namespace CurvesForAge.Views;

public partial class ResultPage : ContentPage
{
    private readonly ResultViewModel _viewModel;
    public ResultPage(DataForAgesRequest request)
    {
        
        InitializeComponent();
        _viewModel = new ResultViewModel(request);
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDataAsync();
    }
}