using System.Text;
using ToDoListApp.Data;
using ToDoListApp.Models;
using ToDoListApp.Views;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Http;
using static ToDoListApp.ToastService;
using System.Net.Mail;
using System.Net;
using Plugin.Maui.Biometric;

namespace ToDoListApp;

public partial class Settings : ContentPage
{
    public bool IsDarkMode = Application.Current.RequestedTheme == AppTheme.Dark;

    public bool IsBioAuthEnabled = Preferences.Get("BiometricsEnabled", false);

    public bool hasErrorShown = false;

    public string runtimeOS = DeviceInfo.Platform.ToString();

    private bool IsConnectedToInternet()
    {
        var current = Connectivity.NetworkAccess;

        if (current == NetworkAccess.Internet)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

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

        if (IsBioAuthEnabled)
        {
            BiometricsSwitch.IsToggled = true;
        }

        else
        {
            BiometricsSwitch.IsToggled = false;
        }
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        VersionTracker();
        await CheckBiometricsStatus();
    }

    private void VersionTracker()
    {
        formattedVersionInfo.Text = $"Checkmate {runtimeOS} Version {VersionTracking.Default.CurrentVersion} ({VersionTracking.Default.CurrentBuild})";
    }

    // Dark Mode
    private async void DarkMode(object sender, EventArgs e)
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
            // Revert Switch
            DarkModeSwitch.IsToggled = !DarkModeSwitch.IsToggled;
            await DisplayAlert("Error", "An error occurred while toggling dark mode." + ex.Message, "Cancel");
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
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PushAsync(new Welcome());
                await ShowToastAsync("Application has been reset", 16, ToastDuration.Short);
            });
        }
    }

    private async Task<byte[]> ReadFileFromOnlineSource(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    if (!hasErrorShown)
                    {
                        await DisplayAlert("Error", "File not found at " + url, "OK");
                        hasErrorShown = true;
                    }
                    return null;
                }
                else if (IsConnectedToInternet() == false)
                {
                    if (!hasErrorShown)
                    {
                        await DisplayAlert("No Internet", "Device not connected to the internet", "OK");
                        hasErrorShown = true;
                    }
                    return null;
                }
                else
                {
                    return null;
                }
            }

            catch (Exception ex)
            {
                if (!hasErrorShown)
                {
                    await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
                    hasErrorShown = true;
                }
                return null;
            }
        }
    }

    private async Task MakeDummyData(bool includeAttachments)
    {
        TodoitemDatabase database = await TodoitemDatabase.Instance;

        string[] priorities = { "Low", "Medium", "High", "Critical" };

        Random random = new Random();

        for (int i = 0; i < 10; i++)
        {
            var item = new Todoitem
            {
                Name = $"Task {i + 1}",
                Notes = $"Description {i + 1}",
                Priority = priorities[random.Next(priorities.Length)],
                Date = DateTime.Now.AddDays(-i),
                Done = i % 2 == 0,
                IsPinned = i % 2 == 0
            };

            if (includeAttachments && i % 3 == 0)
            {
                var attachment = await ReadFileFromOnlineSource("https://cdn-icons-png.flaticon.com/512/1721/1721929.png");

                item.Attachment = attachment;

                if (hasErrorShown)
                {
                    // Reset flag
                    //hasErrorShown = false;
                    break;
                }
            }

            await database.SaveItemAsync(item);
        }
    }

    private async void GenerateData_Button_Pressed(System.Object sender, System.EventArgs e)
    {
        var titlestring = "Attachments require internet access.";
        string action = await Application.Current.MainPage.DisplayActionSheet(titlestring, "Cancel", null, "With Attachments", "Without Attachments");

        if (action == "With Attachments")
        {
            await MakeDummyData(true);
            HapticFeedback.Perform(HapticFeedbackType.Click);

            if (hasErrorShown)
            {
                await ShowToastAsync("Data Not Generated ❎", 16, ToastDuration.Short);
                hasErrorShown = false;
            }
            else
            {
                await ShowToastAsync("Data Generated ✅", 16, ToastDuration.Short);
            }
        }
        else if (action == "Without Attachments")
        {
            await MakeDummyData(false);
            HapticFeedback.Perform(HapticFeedbackType.Click);
            await ShowToastAsync("Data Generated ✅", 16, ToastDuration.Short);
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

    private async void Home_Clicked(System.Object sender, System.EventArgs e)
    {
        Uri appRepo = new("https://github.com/OudomMunint/.NetMAUI-To-Do-List-App");
        await OpenLinks(appRepo);
    }

    private async void Feedback_Clicked(System.Object sender, System.EventArgs e)
    {
        Uri issues = new("https://github.com/OudomMunint/.NetMAUI-To-Do-List-App/issues");
        await OpenLinks(issues);
    }

    private async void AboutMe_Clicked(System.Object sender, System.EventArgs e)
    {
        Uri github = new("https://github.com/OudomMunint");
        Uri portfolio = new("https://oudommunint.netlify.app");

        bool result = await DisplayAlert("About Me", "Do you want to visit my GitHub or Portfolio?", "GitHub", "Portfolio");
        await OpenLinks(result ? github : portfolio);
    }

    async void SeePrevious_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        Uri previousVersions = new("https://github.com/OudomMunint/.NetMAUI-To-Do-List-App/releases");
        await OpenLinks(previousVersions);
    }

    private async Task OpenLinks(Uri url)
    {
        try
        {
            Uri uri = url;
            BrowserLaunchOptions options = new BrowserLaunchOptions()
            {
                LaunchMode = BrowserLaunchMode.SystemPreferred,
                TitleMode = BrowserTitleMode.Show,
                PreferredToolbarColor = Colors.WhiteSmoke,
                PreferredControlColor = Colors.Blue
            };
            await Browser.OpenAsync(uri, options);

        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.ToString(), "Cancel");
        }
    }

    private async Task CheckBiometricsStatus()
    {
        var biometric = BiometricAuthenticationService.Default;
        var enrolledTypes = await biometric.GetEnrolledBiometricTypesAsync();

        if (enrolledTypes.Count() > 0 && enrolledTypes[0] != BiometricType.None)
        {
            BiometricsSection.IsEnabled = true;
        }
        else
        {
            BiometricsSection.IsEnabled = false;
        }
    }

    private async void BiometricsSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        if (BiometricsSwitch.IsToggled)
        {
            Preferences.Set("BiometricsEnabled", true);
            await ShowToastAsync("Biometrics Enabled", 20, ToastDuration.Short);
        }
        else
        {
            Preferences.Set("BiometricsEnabled", false);
            await ShowToastAsync("Biometrics Disabled", 20, ToastDuration.Short);
        }
    }

    private void Button_Pressed(object sender, EventArgs e)
    {
        // Debug pref
        var BioPref = Preferences.Get("BiometricsEnabled", false);
        Console.WriteLine(BioPref);
    }
}