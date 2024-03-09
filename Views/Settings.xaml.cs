using Microsoft.VisualBasic.FileIO;
using ToDoListApp.Data;
using ToDoListApp.Views;

namespace ToDoListApp;

public partial class Settings : ContentPage
{
    public bool IsDarkMode = Application.Current.RequestedTheme == AppTheme.Dark;

    public Settings()
	{
        InitializeComponent();

        if (IsDarkMode)
        {
            DarkModeSwitch.IsToggled = true;
        }

        else
        {
            DarkModeSwitch.IsToggled = false;
        }
	}

    // Dark Mode
    private void DarkMode(object sender, EventArgs e)
    {
        if (DarkModeSwitch.IsToggled)
        {
            Application.Current.UserAppTheme = AppTheme.Dark;
        }

        else
        {
            Application.Current.UserAppTheme = AppTheme.Light;
        }
    }

    async void Reset_Button_Pressed(System.Object sender, System.EventArgs e)
    {
        var userConfirmed = await DisplayAlert("Reset Application", "This action will delete existing tasks and reset this app. Are you sure you want to continue", "Yes", "No");

        if (userConfirmed)
        {
            var database = await TodoitemDatabase.Instance;
            var allitems = await database.GetItemsAysnc();
            foreach (var item in allitems)
            {
                await database.DeleteItemAsync(item);
            }
            // Navigate with main thread!!!
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Navigation.PushAsync(new Welcome());
            });
        }
    }

    async void Go_To_Welcome_Button_Pressed(System.Object sender, System.EventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Navigation.PushAsync(new Welcome());
            Navigation.RemovePage(this);
        });
    }
}