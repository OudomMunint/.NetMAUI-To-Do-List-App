using ToDoListApp.Data;
using ToDoListApp.Models;
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

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    // Dark Mode
    private void DarkMode(object sender, EventArgs e)
    {
        try
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
        catch (Exception ex)
        {
            // Handle ex
            Console.WriteLine($"An error occurred while toggling dark mode: {ex.Message}");
            // Revert Switch
            DarkModeSwitch.IsToggled = !DarkModeSwitch.IsToggled;
            // Error message
            DisplayAlert("Error", "An error occurred while toggling dark mode.", "Cancel");
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
    private static async Task MakeDummyData()
        {
            TodoitemDatabase database = await TodoitemDatabase.Instance;

            string[] priorities = ["Low", "Medium", "High", "Critical"];

            Random random = new();

            for (int i = 0; i < 10; i++)
            {
                var item = new Todoitem
                {
                    Name = $"Task {i + 1}",
                    Notes = $"Description {i + 1}",
                    Priority = priorities[random.Next(priorities.Length)],
                    Date = DateTime.Now.AddDays(i),
                    Done = i % 2 == 0
                };

                await database.SaveItemAsync(item);
            }
        }

    private async void GenerateData_Button_Pressed(System.Object sender, System.EventArgs e)
    {
        // Alert
        var userConfirmed = await DisplayAlert("Generate Dummy Data", "This action will generate dummy data and will affect existing To-Do items. Are you sure you want to continue", "Yes", "No");

        if (userConfirmed)
        {
            await Settings.MakeDummyData();
            await DisplayAlert("Success", "Dummy data has been generated", "OK");
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