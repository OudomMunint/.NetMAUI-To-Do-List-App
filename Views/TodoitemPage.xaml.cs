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
        public TodoitemPage()
        {
            InitializeComponent();

            Switch[] switches = { DoneSwitch, PinSwitch };

            UiHelpers.SetSwitchColors(switches);
        }

        public enum SamplingQuality
        {
            Low,
            Medium,
            Mitchell,
            CatMullRom
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateToolbarTitle();
            GetAttachmentCount();
            SetSelectedPriorityLabel();
            GetAttachmentSize();
        }

        public static SKSamplingOptions GetSamplingOptions(SamplingQuality quality)
        {
            return quality switch
            {
                SamplingQuality.Low => new SKSamplingOptions(SKFilterMode.Nearest, SKMipmapMode.Nearest),
                SamplingQuality.Medium => new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear),
                SamplingQuality.Mitchell => new SKSamplingOptions(SKCubicResampler.Mitchell),
                SamplingQuality.CatMullRom => new SKSamplingOptions(SKCubicResampler.CatmullRom),
                _ => throw new NotImplementedException(),
            };
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

            // Null Check
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

        public async void TakePhoto(object sender, EventArgs e)
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                    if (photo != null)
                    {
                        // save the file into local storage
                        string localFilePath = Path.Combine(Microsoft.Maui.Storage.FileSystem.CacheDirectory, photo.FileName);

                        using (Stream sourceStream = await photo.OpenReadAsync())
                        {
                            using (FileStream localFileStream = File.OpenWrite(localFilePath))
                            {
                                await sourceStream.CopyToAsync(localFileStream);
                            }
                        }

                        // show the photo in the UI
                        var todoItem = (Todoitem)BindingContext;
                        TodoitemDatabase database = await TodoitemDatabase.Instance;

                        // subsample attached
                        byte[] originalBytes = File.ReadAllBytes(localFilePath);
                        todoItem.Attachment = SubSampleImageToByteArray(originalBytes, 700);

                        await Task.Delay(1000);

                        attachmentImage.Source = ImageSource.FromStream(() => new MemoryStream(todoItem.Attachment));
                        var AttachmentSize = todoItem.Attachment.Length / 1000;
                        attsize.Text = AttachmentSize.ToString() + " KB";
                        attlabel.IsVisible = false;
                    }
                }
            }
            catch (Exception exception)
            {
                await DisplayAlert("Camera Error", exception.ToString(), "OK");
            }
        }

        public async void UploadPhoto(object sender, EventArgs e)
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                FileResult uploadedFile = await MediaPicker.Default.PickPhotoAsync();

                if (uploadedFile != null)
                {
                    // save the file into local storage
                    string localFilePath = Path.Combine(Microsoft.Maui.Storage.FileSystem.CacheDirectory, uploadedFile.FileName);

                    using (Stream sourceStream = await uploadedFile.OpenReadAsync())
                    {
                        using (FileStream localFileStream = File.OpenWrite(localFilePath))
                        {
                            await sourceStream.CopyToAsync(localFileStream);
                        }
                    }

                    // show the photo in the UI
                    var todoItem = (Todoitem)BindingContext;
                    TodoitemDatabase database = await TodoitemDatabase.Instance;

                    // subsample attached
                    byte[] originalBytes = File.ReadAllBytes(localFilePath);
                    todoItem.Attachment = SubSampleImageToByteArray(originalBytes, 700);

                    await Task.Delay(1000);

                    attachmentImage.Source = ImageSource.FromStream(() => new MemoryStream(todoItem.Attachment));
                    var AttachmentSize = todoItem.Attachment.Length / 1000;
                    attsize.Text = AttachmentSize.ToString() + " KB";
                    attlabel.IsVisible = false;
                }
            }
        }

        private static byte[] SubSampleImageToByteArray(byte[] originalBytes, int maxWidth)
        {
            using var inputStream = new MemoryStream(originalBytes);
            using var codec = SKCodec.Create(inputStream);
            if (codec == null)
                return originalBytes;

            // Get the EXIF orientation
            var orientation = codec.EncodedOrigin;
            // Decode to bitmap
            using var original = SKBitmap.Decode(codec);
            if (original == null)
                return originalBytes;

            // OG dimensions
            int originalWidth = original.Width;
            int originalHeight = original.Height;

            // Maintaine aspect ratio
            float ratio = (float)maxWidth / originalWidth;
            int newWidth = maxWidth;
            int newHeight = (int)(originalHeight * ratio);

            var samplingOptions = GetSamplingOptions(SamplingQuality.Medium);
            using var resized = original.Resize(new SKImageInfo(newWidth, newHeight), samplingOptions);
            if (resized == null)
                return originalBytes;

            SKBitmap rotated;

            // Handle rotation
            switch (orientation)
            {
                case SKEncodedOrigin.RightTop: // 90°
                    rotated = new SKBitmap(resized.Height, resized.Width);
                    using (var canvas = new SKCanvas(rotated))
                    {
                        canvas.Translate(rotated.Width, 0);
                        canvas.RotateDegrees(90);
                        canvas.DrawBitmap(resized, 0, 0);
                    }
                    break;

                case SKEncodedOrigin.BottomRight: // 180°
                    rotated = new SKBitmap(resized.Width, resized.Height);
                    using (var canvas = new SKCanvas(rotated))
                    {
                        canvas.Translate(rotated.Width, rotated.Height);
                        canvas.RotateDegrees(180);
                        canvas.DrawBitmap(resized, 0, 0);
                    }
                    break;

                case SKEncodedOrigin.LeftBottom: // 270°
                    rotated = new SKBitmap(resized.Height, resized.Width);
                    using (var canvas = new SKCanvas(rotated))
                    {
                        canvas.Translate(0, rotated.Height);
                        canvas.RotateDegrees(270);
                        canvas.DrawBitmap(resized, 0, 0);
                    }
                    break;

                default:
                    rotated = resized;
                    break;
            }

            // Bitmap => image
            using var image = SKImage.FromBitmap(rotated);
            using var output = new MemoryStream();
            // 70 = compression quality (0-100), lower = smaller file size but more compression artifacts
            // Note: if using quality < 70, consider using better resampling via SamplingQuality.Mitchell or SamplingQuality.CatmullRom
            image.Encode(SKEncodedImageFormat.Jpeg, 70).SaveTo(output);

            return output.ToArray();
        }

        public async void OpenMenu(object sender, EventArgs e)
        {
            string delete = "Delete Item";
            string clear = "Clear Form";

            var action = await Application.Current.MainPage.DisplayActionSheet(null, "Cancel", null, new[] { delete, clear });

            if (action != null && action.Equals(delete))
            {
                OnDeleteClicked(sender, e);
            }
            else if (action != null && action.Equals(clear))
            {
                OnClearClicked(sender, e);
            }
        }

        public async void AttMenu_Clicked(object sender, EventArgs e)
        {
            string addatachment = "Take Photo";
            string uploadatachment = "Upload Attachment";

            var action = await Application.Current.MainPage.DisplayActionSheet(null, "Cancel", null, new[] { addatachment, uploadatachment });

            if (action != null && action.Equals(addatachment))
            {
                TakePhoto(sender, e);
            }
            else if (action != null && action.Equals(uploadatachment))
            {
                UploadPhoto(sender, e);
            }
        }
    }
}