namespace ToDoListApp;

public partial class Settings : ContentPage
{
    public Settings()
	{
        InitializeComponent();
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