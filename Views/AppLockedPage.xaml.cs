using Plugin.Maui.Biometric;
using static ToDoListApp.ToastService;

namespace ToDoListApp.Views;

public partial class AppLockedPage : ContentPage
{
    private bool isAuthenticated = false;

    public AppLockedPage()
	{
		InitializeComponent();
	}

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (isAuthenticated)
        {
            Navigation.RemovePage(this);
        }
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
            NegativeText = "Cancel"
        }, CancellationToken.None);

        if (biometric.Status == BiometricResponseStatus.Success)
        {
            isAuthenticated = true;
            await Navigation.PushAsync(new Dashboard());
        }
        else
        {
            isAuthenticated = false;
            await ShowToastAsync("Authentication failed", 20, CommunityToolkit.Maui.Core.ToastDuration.Short);
        }
    }
}