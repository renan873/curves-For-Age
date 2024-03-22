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
        #if __ANDROID__
        AdMobBanner.AdsId = "ca-app-pub-5964431097928350/8012461541";
        #elif __IOS__
        AdMobBanner.AdsId = "ca-app-pub-5964431097928350/8501763749";
        #endif
    }

}