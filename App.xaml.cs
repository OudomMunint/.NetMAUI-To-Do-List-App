using ToDoListApp.Views;
using Microsoft.Maui.Controls;

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
}