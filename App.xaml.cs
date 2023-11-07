using ToDoListApp.Views;
using Microsoft.Maui.Controls;

namespace ToDoListApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        if (Preferences.ContainsKey("IsFirstRun"))
        {
            MainPage = new NavigationPage(new TodoListPage());
        }
        else
        {
            MainPage = new NavigationPage(new Welcome());
            Preferences.Set("IsFirstRun", true);
        }
    }
}