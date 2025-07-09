using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ToDoListApp.Data;
using ToDoListApp.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using ToDoListApp.Helpers;
using static ToDoListApp.ToastService;
using SkiaSharp;

namespace ToDoListApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TodoitemPage : ContentPage
    {
        private bool shouldProcessAttachment = false;

        public TodoitemPage()
        {
            InitializeComponent();
            Switch[] switches = { DoneSwitch, PinSwitch, DownSizeSwitch };
            UiHelpers.SetSwitchColors(switches);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateToolbarTitle();
            GetAttachmentCount();
            SetSelectedPriorityLabel();
            GetAttachmentSize();
        }

        private void GetAttachmentSize()
        {
            var todoItem = (Todoitem)BindingContext;

            if (todoItem.Attachment != null)
            {
                var AttachmentSize = todoItem.Attachment.Length / 1000;
                attsize.Text = AttachmentSize.ToString() + " KB";
            }
        }

        private void SetSelectedPriorityLabel()
        {
            var todoItem = (Todoitem)BindingContext;

            if (todoItem.Priority == "Low")
            {
                PriorityPicker.Title = "Low";
                PriorityPicker.SelectedIndex = 0;
            }
            else if (todoItem.Priority == "Medium")
            {
                PriorityPicker.Title = "Medium";
                PriorityPicker.SelectedIndex = 1;
            }
            else if (todoItem.Priority == "High")
            {
                PriorityPicker.Title = "High";
                PriorityPicker.SelectedIndex = 2;
            }
            else if (todoItem.Priority == "Critical")
            {
                PriorityPicker.Title = "Critical";
                PriorityPicker.SelectedIndex = 3;
            }
            else
            {
                PriorityPicker.Title = "Select Priority";
            }
        }

        private void GetAttachmentCount()
        {
            var todoItem = (Todoitem)BindingContext;

            if (todoItem.Attachment != null)
            {
                var attachmentCount = todoItem.Attachment.Count();

                if (attachmentCount > 0)
                {
                    attlabel.IsVisible = false;
                }
                else
                {
                    attlabel.IsVisible = true;
                }
            }
            else
            {
                attlabel.IsVisible = true;
            }
        }

        private void UpdateToolbarTitle()
        {
            if (string.IsNullOrWhiteSpace(NameField.Text))
            {
                itemTitle.FormattedText = new FormattedString();
                itemTitle.FormattedText.Spans.Add(new Span { Text = "Create task", FontAttributes = FontAttributes.Bold });
            }
            else
            {
                itemTitle.FormattedText = new FormattedString();
                itemTitle.FormattedText.Spans.Add(new Span { Text = "Editing", FontAttributes = FontAttributes.Bold });
                //itemTitle.FormattedText.Spans.Add(new Span { Text = NameField.Text });
            }
        }

        private async void DeleteAttachmentClicked(object sender, EventArgs e)
        {
            var todoItem = (Todoitem)BindingContext;
            TodoitemDatabase database = await TodoitemDatabase.Instance;

            // if null
            if (todoItem.Attachment == null)
            {
                await DisplayAlert("No Attachment", "There is no attachment to delete", "OK");
                return;
            }

            // Alert
            bool Confirmed = await DisplayAlert("Delete Attachment.", "Are you sure you want to delete this Attachment?", "Yes", "No");

            if (Confirmed)
            {
                todoItem.Attachment = null;
                attlabel.IsVisible = true;
                attachmentImage.Source = null;
                attsize.Text = null;

                // Save the updated todoItem to the database
                await database.SaveItemAsync(todoItem);
            }
        }

        async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameField.Text) || string.IsNullOrWhiteSpace(DescField.Text))
            {
                HapticFeedback.Perform(HapticFeedbackType.LongPress);
                await DisplayAlert("Cannot Save Task", "Please enter Name and Notes", "OK");
                return;
            }

            var todoItem = (Todoitem)BindingContext;
            todoItem.Name = NameField.Text;
            todoItem.Notes = DescField.Text;
            todoItem.Date = DateTime.Now;
            todoItem.Priority = PriorityPicker.SelectedIndex.ToString();

            if (todoItem.Priority == "0")
            {
                todoItem.Priority = "Low";
            }
            else if (todoItem.Priority == "1")
            {
                todoItem.Priority = "Medium";
            }
            else if (todoItem.Priority == "2")
            {
                todoItem.Priority = "High";
            }
            else if (todoItem.Priority == "3")
            {
                todoItem.Priority = "Critical";
            }
            else
            {
                todoItem.Priority = "";
            }

            HapticFeedback.Perform(HapticFeedbackType.Click);
            TodoitemDatabase database = await TodoitemDatabase.Instance;

            await database.SaveItemAsync(todoItem);
            await ShowToastAsync("Task Saved ✅", 16, ToastDuration.Long);
            await Navigation.PopAsync();
        }

        async void OnDeleteClicked(object sender, EventArgs e)
        {
            HapticFeedback.Perform(HapticFeedbackType.LongPress);
            bool Confirmed = await DisplayAlert("Delete To-Do Item.", "Are you sure you want to delete this item? all attachments bound to this item will be lost", "Yes", "No");

            if (Confirmed)
            {
                HapticFeedback.Perform(HapticFeedbackType.Click);
                var todoItem = (Todoitem)BindingContext;
                TodoitemDatabase database = await TodoitemDatabase.Instance;

                await database.DeleteItemAsync(todoItem);
                await ShowToastAsync("Task Deleted 🗑️", 16, ToastDuration.Short);
                await Navigation.PopAsync();
            }
        }

        private async void OnClearClicked(object sender, EventArgs e)
        {
            NameField.Text = " ";
            DescField.Text = " ";
            attachmentImage.Source = null;
            HapticFeedback.Perform(HapticFeedbackType.Click);
            await ShowToastAsync("Form Cleared ⌫", 16, ToastDuration.Short);
        }

        public async void OpenMenu(object sender, EventArgs e)
        {
            string delete = "Delete Item";
            string clear = "Clear Form";

            var action = await Application.Current?.Windows[0].Page.DisplayActionSheet(null, "Cancel", null, new[] { delete, clear });

            if (action != null && action.Equals(delete))
            {
                OnDeleteClicked(sender, e);
            }
            else if (action != null && action.Equals(clear))
            {
                OnClearClicked(sender, e);
            }
        }

        private async void TakePhoto(object sender, EventArgs e)
        {
            var result = await PhotoHelper.TakePhotoAsync(shouldProcessAttachment);
            if (result.Success)
            {
                var todoItem = (Todoitem)BindingContext;
                todoItem.Attachment = result.ImageData;
                attachmentImage.Source = ImageSource.FromStream(() => new MemoryStream(todoItem.Attachment));
                attlabel.IsVisible = false;
                GetAttachmentSize();
                GetAttachmentCount();
            }
            else
            {
                await DisplayAlert("Error", result.ErrorMessage, "OK");
            }
        }

        private async void UploadPhoto(object sender, EventArgs e)
        {
            var result = await PhotoHelper.UploadPhotoAsync(shouldProcessAttachment);
            if (result.Success)
            {
                var todoItem = (Todoitem)BindingContext;
                todoItem.Attachment = result.ImageData;
                attachmentImage.Source = ImageSource.FromStream(() => new MemoryStream(todoItem.Attachment));
                attlabel.IsVisible = false;
                GetAttachmentSize();
                GetAttachmentCount();
            }
            else
            {
                await DisplayAlert("Error", result.ErrorMessage, "OK");
            }
        }

        public async void AttMenu_Clicked(object sender, EventArgs e)
        {
            string addatachment = "Take Photo";
            string uploadatachment = "Upload Attachment";

            var action = await Application.Current?.Windows[0].Page.DisplayActionSheet(null, "Cancel", null, new[] { addatachment, uploadatachment });

            if (action != null && action.Equals(addatachment))
            {
                TakePhoto(sender, e);
            }
            else if (action != null && action.Equals(uploadatachment))
            {
                UploadPhoto(sender, e);
            }
        }

        private void DownSizeSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                shouldProcessAttachment = true;
            }
            else
            {
                shouldProcessAttachment = false;
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            DisplayAlert("Reduce Attachment Size", "Reduces file size but lowers photo quality, applied next time a photo is attached.", "OK");
        }
    }
}