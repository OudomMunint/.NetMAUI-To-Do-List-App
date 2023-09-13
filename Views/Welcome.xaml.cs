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
}