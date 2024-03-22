using CurvesForAge.Models;
using CurvesForAge.ViewModels;

namespace CurvesForAge.Views;

public partial class ResultPage : ContentPage
{
    private readonly ResultViewModel _viewModel;
    public ResultPage(DataForAgesRequest request)
    {
        InitializeComponent();
        SetBannerId();
        _viewModel = new ResultViewModel(request);
        BindingContext = _viewModel;
    }

    private void SetBannerId()
    {
#if __ANDROID__
        AdMobBanner.AdsId = "ca-app-pub-5964431097928350/9027152593";
#elif __IOS__
        AdMobBanner.AdsId = "ca-app-pub-5964431097928350/9027152593";
#endif
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDataAsync();
    }
}