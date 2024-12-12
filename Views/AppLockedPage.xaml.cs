using Plugin.Maui.Biometric;
using static ToDoListApp.ToastService;

namespace ToDoListApp.Views;

public partial class AppLockedPage : ContentPage
{
	public AppLockedPage()
	{
		InitializeComponent();
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
            await Navigation.PushAsync(new Dashboard());
        }
        else
        {
            await ShowToastAsync("Authentication failed", 20, CommunityToolkit.Maui.Core.ToastDuration.Short);
        }
    }
}