using ToDoListApp.Views;
using Microsoft.Maui.Controls;
using Plugin.Maui.Biometric;
using static ToDoListApp.ToastService;
#if IOS
using WebKit;
#endif

namespace ToDoListApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        bool isBiometricsEnabled = Preferences.Get("BiometricsEnabled", false);

        if (Preferences.ContainsKey("IsFirstRun"))
        {
            MainPage = new NavigationPage(new Dashboard());
        }
        else
        {
            MainPage = new NavigationPage(new Welcome());
            Preferences.Set("IsFirstRun", true);
        }

        if (isBiometricsEnabled)
        {
            MainPage = new NavigationPage(new AppLockedPage());
        }
    }

    protected override async void OnResume()
    {
        base.OnResume();

#if ANDROID
        await Task.Delay(500); // Android needs this delay or authentication will be bypassed
#endif

        bool isBiometricsEnabled = Preferences.Get("BiometricsEnabled", false);

        if (isBiometricsEnabled == true)
        {
            // Biometric authentication
            var biometric = await BiometricAuthenticationService.Default.AuthenticateAsync(new AuthenticationRequest()
            {
                Title = "Authenticate",
                Subtitle = "Please authenticate to access the app",
                Description = "Please use your biometric to authenticate",
                NegativeText = "Cancel",
                AuthStrength = AuthenticatorStrength.Weak,
                AllowPasswordAuth = true

            }, CancellationToken.None);

            if (biometric.Status == BiometricResponseStatus.Failure)
            {
                await MainPage.Navigation.PushAsync(new AppLockedPage());
            }
        }
    }
}