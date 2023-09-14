using ToDoListApp.Views;

namespace ToDoListApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new NavigationPage(new Welcome())
		{
			//BarTextColor = Color.FromRgb(255, 2555, 255)
		};
	}
}
