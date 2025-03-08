using System.Collections.ObjectModel;

namespace ToDoListApp.Views;

public partial class Welcome : ContentPage
{
    public class WelcomeCarousel
    {
        public ImageSource WelcomeImage { get; set; }
    }

    private ObservableCollection<WelcomeCarousel> welcomeCollection;

    public ObservableCollection<WelcomeCarousel> WelcomeCollection
    {
        get { return welcomeCollection; }
        set
        {
            welcomeCollection = value;
            OnPropertyChanged();
        }
    }
    
    public Welcome()
    {
        InitializeComponent();
        SetCarouselImages();
        BindingContext = this;
        this.Loaded += Welcome_Loaded;

        if (CarouselMain.Position == 0)
        {
            ChangingText.Text = "Managing your task made easy";
            ChangingSubText.Text = "You can view your stats with an informative dashboard";
        }
    }

    private void SetCarouselImages()
    {
        WelcomeCollection = new ObservableCollection<WelcomeCarousel>
        {
            new() {WelcomeImage = "dashboard.png"},
            new() {WelcomeImage = "iphone2.png"},
            new() {WelcomeImage = "iphone3.png"},
            new() {WelcomeImage = "iphone4.png"},
            new() {WelcomeImage = "iphone5.png"},
        };

        CarouselMain.ItemsSource = WelcomeCollection;
    }

    private void Welcome_Loaded(object sender, EventArgs e)
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

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Task.Delay(500);
        RemoveSettingsPage();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        this.Loaded -= Welcome_Loaded;
    }

    private void Continue_Clicked(object sender, EventArgs e)
    {
        try
        {
            Application.Current.MainPage = new AppShell();
            // Prevent the user from going back
            // Navigation.RemovePage(this); // Disabled for now due to #236
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void OnPositionChanged(object sender, PositionChangedEventArgs e)
    {
        ChangingText.Text = e.CurrentPosition switch
        {
            0 => "Managing your task made easy",
            1 => "Add new tasks or mark it as done",
            2 => "Bulk actions supported",
            3 => "Settings and more",
            4 => "Biometrics with FaceID or TouchID",
            _ => string.Empty,
        };

        ChangingSubText.Text = e.CurrentPosition switch
        {
            0 => "You can view your stats with an informative dashboard.",
            1 => "Click on + to add a new task or click on the task to edit it.",
            2 => "Perform bulk actions like delete, mark as done, set priority.",
            3 => "Give feedback, switch themes or generate some data to get started.",
            4 => "Go to settings and enable Biometrics to start securing you app.",
            _ => string.Empty,
        };
    }

    // Remove settings page from navigation stack if navigated from settings page
    private void RemoveSettingsPage()
    {
        var navigationStack = Navigation.NavigationStack;
        if (navigationStack.Count > 1 && navigationStack[navigationStack.Count - 2] is Settings)
        {
            Navigation.RemovePage(navigationStack[navigationStack.Count - 2]);
        }
    }

}