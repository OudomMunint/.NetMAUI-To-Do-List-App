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
        await Navigation.PushAsync(new Welcome());
    }
    //private void OnCounterClicked(object sender, EventArgs e)
    //{
    //	count++;

    //	if (count == 1)
    //		CounterBtn.Text = $"Clicked {count} time";
    //	else
    //		CounterBtn.Text = $"Clicked {count} times";

    //	SemanticScreenReader.Announce(CounterBtn.Text);
    //}
}