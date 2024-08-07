﻿using System.Text;
using ToDoListApp.Data;
using ToDoListApp.Models;
using ToDoListApp.Views;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.ComponentModel.DataAnnotations.Schema;
using static ToDoListApp.ToastService;

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

    protected override void OnAppearing()
    {
        base.OnAppearing();
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
    private static async Task MakeDummyData()
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
                IsPinned = i % 3 == 0,
                HasAttachment = i % 4 == 0 //Doesn't actually generate Images.
            };

            await database.SaveItemAsync(item);
        }
    }

    private async void GenerateData_Button_Pressed(System.Object sender, System.EventArgs e)
    {
        // Alert
        var userConfirmed = await DisplayAlert("Generate Dummy Data", "This action will generate dummy data and will affect existing To-Do items. Are you sure you want to continue", "Yes", "No");

        if (userConfirmed)
        {
            try
            {
                await MakeDummyData();
                HapticFeedback.Perform(HapticFeedbackType.Click);
                await ShowToastAsync("Data Generated ✅", 16, ToastDuration.Short);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "Ok");
            }
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
        try
        {
            Uri uri = new("https://github.com/OudomMunint/.NetMAUI-To-Do-List-App");
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.ToString(), "Cancel");
        }
    }

    private async void Feedback_Clicked(System.Object sender, System.EventArgs e)
    {
        try
        {
            Uri uri = new("https://github.com/OudomMunint/.NetMAUI-To-Do-List-App/issues");
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.ToString(), "Cancel");
        }
    }

    private async void AboutMe_Clicked(System.Object sender, System.EventArgs e)
    {
        try
        {
            Uri uri = new("https://github.com/OudomMunint");
            Uri uri2 = new("https://oudommunint.netlify.app");

            bool result = await DisplayAlert("About Me", "Do you want to visit my GitHub or Portfolio?", "GitHub", "Portfolio");
            await Browser.Default.OpenAsync(result ? uri : uri2, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.ToString(), "Cancel");
        }
    }
}