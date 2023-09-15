using System.Collections.ObjectModel;

namespace ToDoListApp.Views;

public partial class Welcome : ContentPage
{
    private int itemcount;

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
            };

        itemcount = UserCollection.Count;

        if (CarouselZoos.Position == 0)
        {
            ChangingText.Text = "Managing your task made easy";
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
        Navigation.PushAsync(new TodoListPage());

        // Prevent the user from going back
        Navigation.RemovePage(this);
    }

    private void OnPositionChanged(object sender, PositionChangedEventArgs e)
    {
        if (e.CurrentPosition == 0)
        {
            ChangingText.Text = "Managing your task made easy";
        }

        if (e.CurrentPosition == 1)
        {
            ChangingText.Text = "Add new tasks or mark it as done ";
        }

        else if (e.CurrentPosition == 2)
        {
            ChangingText.Text = "Bulk marking or delete them";
        }

        else if (e.CurrentPosition == 3)
        {
            ChangingText.Text = "Change theme settings";
        }
    }
}