using System.Collections.ObjectModel;

namespace ToDoListApp.Views;

public partial class Welcome : ContentPage
{
    public Welcome()
	{
		InitializeComponent();
        BindingContext = this;
        UserCollection = new ObservableCollection<UserInformation>
        {
            new UserInformation{UserImage = "iphone1.png"},
            new UserInformation{UserImage = "iphone2.png"},
            new UserInformation{UserImage = "iphone3.png"},
            new UserInformation{UserImage = "iphone4.png"},
            new UserInformation{UserImage = "iphone5.png"},
        };

        if (CarouselZoos.Position == 0)
        {
            ChangingText.Text = "Managing your task made easy";
            ChangingSubText.Text = "You can view your stats with an informative dashboard";
        }
    }

    public class UserInformation
    {
        public ImageSource UserImage { get; set; }
    }

    private ObservableCollection<UserInformation> userCollection;

    public ObservableCollection<UserInformation> UserCollection
    {
        get { return userCollection; }
        set
        {
            userCollection = value;
            OnPropertyChanged();
        }
    }

    private void Continue_Clicked(object sender, EventArgs e)
    {
        try
        {
            Navigation.PushAsync(new Dashboard());
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
}