using Plugin.Maui.Biometric;
using static ToDoListApp.ToastService;

namespace ToDoListApp.Views;

public partial class AppLockedPage : ContentPage
{
    private bool isAuthenticated = false;

    public AppLockedPage()
    {
        InitializeComponent();
        this.Loaded += AppLockedPage_Loaded;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        //if (isAuthenticated)
        //{
        //    Navigation.RemovePage(this);
        //}

        this.Loaded -= AppLockedPage_Loaded;
    }

    private void AppLockedPage_Loaded(object sender, EventArgs e)
    {
#if IOS
        if (Handler is IPlatformViewHandler platformViewHandler)
        {
            var controller = platformViewHandler.ViewController?.NavigationController;
            if (platformViewHandler.ViewController?.NavigationController != null)
            {
                platformViewHandler.ViewController.NavigationController.InteractivePopGestureRecognizer.Enabled = false;
            }
        }
#endif
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }

    private async void Authenticate_Clicked(object sender, EventArgs e)
    {
        await Authenticate();
    }

    private async Task Authenticate()
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

        if (biometric.Status == BiometricResponseStatus.Success)
        {
            isAuthenticated = true;
            Application.Current.MainPage = new AppShell();
        }
        else
        {
            isAuthenticated = false;
            await ShowToastAsync("Authentication failed", 20, CommunityToolkit.Maui.Core.ToastDuration.Short);
        }
    }
}