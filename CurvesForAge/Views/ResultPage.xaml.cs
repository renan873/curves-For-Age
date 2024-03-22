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
        AdMobBanner.AdsId = DeviceInfo.Platform == DevicePlatform.Android
            ? "ca-app-pub-5964431097928350/9027152593"
            : "ca-app-pub-5964431097928350/9027152593";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        AdMobBanner.LoadAd();
        await _viewModel.LoadDataAsync();
    }
}