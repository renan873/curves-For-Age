using CurvesForAge.ViewModels;

namespace CurvesForAge;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        SetBannerId();
        var viewModel = new MainViewModel
        {
            Navigation = Navigation
        };
        BindingContext = viewModel;
    }
    
    private void SetBannerId()
    {
        AdMobBanner.AdsId = DeviceInfo.Platform == DevicePlatform.Android
            ? "ca-app-pub-5964431097928350/8012461541"
            : "ca-app-pub-5964431097928350/8501763749";
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AdMobBanner.LoadAd();
    }
}