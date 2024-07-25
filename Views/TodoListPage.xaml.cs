using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using ToDoListApp.Data;
using ToDoListApp.Models;
using CommunityToolkit.Maui.Core.Platform;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System;

namespace ToDoListApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TodoListPage : ContentPage
    {
        // dark mode property
        private readonly AppTheme darkmode = AppTheme.Dark;

        public TodoListPage()
        {
            InitializeComponent();
            todolistlist = listView;

            NavigationPage.SetBackButtonTitle(this, " ");

            Application.Current.RequestedThemeChanged += (s, a) =>
            {
                if (Application.Current.RequestedTheme == darkmode)
                {
                    listView.BackgroundColor = Colors.Black;
                }

                else
                {
                    listView.BackgroundColor = Colors.White;
                }
            };
        }

        public ListView todolistlist;

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await IsEmptyList();
            await GetItemsWithAttachment();
            await SetPinnedOnlyListSource();
            await UpdateListView();
            await UpdateCollectionView();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private async Task IsEmptyList()
        {
            TodoitemDatabase database = await TodoitemDatabase.Instance;
            var items = await database.GetItemsAysnc();
            var pinneditems = await database.GetItemsPinnedAysnc();

            bool isItemsEmpty = items.Count == 0;
            bool isPinnedItemsEmpty = pinneditems.Count == 0;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                VStack.IsVisible = isItemsEmpty;
                listView.IsVisible = !isItemsEmpty;
                pinnedcontainer.IsVisible = !isPinnedItemsEmpty;
            });
        }

        private async Task GetItemsWithAttachment()
        {
            TodoitemDatabase database = await TodoitemDatabase.Instance;
            var items = await database.GetItemsAysnc();

            foreach (var item in items)
            {
                if (item.Attachment != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        item.HasAttachment = true;
                    });
                    await database.SaveItemAsync(item);
                }

                else
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        item.HasAttachment = false;
                    });
                    await database.SaveItemAsync(item);
                }
            }
        }

        private async Task UpdateListView()
        {
            TodoitemDatabase database = await TodoitemDatabase.Instance;
            listView.ItemsSource = await database.GetItemsAysnc();
            UpdateTitle();
        }

        private async Task UpdateCollectionView()
        {
            TodoitemDatabase database = await TodoitemDatabase.Instance;
            pinnedList.ItemsSource = await database.GetItemsPinnedAysnc();
            await IsEmptyList();
        }

        async void OnItemAdded(object sender, EventArgs e)
        {
            HapticFeedback.Perform(HapticFeedbackType.Click);

            await Navigation.PushAsync(new TodoitemPage
            {
                BindingContext = new Todoitem()
            });
        }

        async void OpenSettings(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Settings());

            // Open BottomSheet
            //var page = new MyBottomSheet();
            //await page.ShowAsync();
        }

        async void DeleteAllItems(object sender, EventArgs e)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            ToastDuration duration = ToastDuration.Short;
            string text2 = "All Task(s) Deleted 🗑";
            var deleteAllToast = Toast.Make(text2, duration, 16);

            bool userConfirmed = await DisplayAlert("Delete All Tasks", "Confirm you want to DELETE ALL items?", "Yes", "No");

            if (userConfirmed)
            {
                TodoitemDatabase database = await TodoitemDatabase.Instance;
                var allitems = await database.GetItemsAysnc();

                foreach (var item in allitems)
                {
                    await database.DeleteItemAsync(item);
                }

                listView.ItemsSource = null;
                await IsEmptyList();
                await UpdateListView();
                try
                {
                    await deleteAllToast.Show(cancellationTokenSource.Token);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.ToString(), "Cancel");
                    Console.WriteLine(ex);
                }
            }
        }

        async void OnDeleteClicked(object sender, EventArgs e)
        {
            var todoItem = (Todoitem)BindingContext;
            TodoitemDatabase database = await TodoitemDatabase.Instance;
            await database.DeleteItemAsync(todoItem);
            await UpdateListView(); // Update ListView after deleting item.
            await Navigation.PopAsync();
        }

        async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            if (e.SelectedItem != null)
            {
                await Navigation.PushAsync(new TodoitemPage
                {
                    BindingContext = e.SelectedItem as Todoitem
                });
            }
        }

        private void UpdateTitle()
        {
            int totalItems = listView.ItemsSource?.Cast<object>().Count() ?? 0;
            string task = "Task";

            // pluralize "task" if totalItems > 1
            if (totalItems != 1)
            {
                task += "s";
            }
            Title = $"🏠 {totalItems} Opened";
            labeltitle.Text = $"🏠 {totalItems} Opened";
        }

        private void OnCheckBoxChecked(object sender, EventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var todoItem = checkBox.BindingContext as Todoitem;
            if (todoItem != null)
            {
                todoItem.IsSelected = checkBox.IsChecked;
            }
        }

        private void OnCheckBoxUnchecked(object sender, EventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var todoItem = checkBox.BindingContext as Todoitem;
            if (todoItem != null)
            {
                todoItem.IsSelected = checkBox.IsChecked;
            }
        }

        async void DeleteSelectedItems(object sender, EventArgs e)
        {
            CancellationTokenSource cancellationTokenSource = new();
            ToastDuration duration = ToastDuration.Short;
            string text = "Selected Task(s) Deleted 🗑️";
            var deleteSelectedToast = Toast.Make(text, duration, 16);

            var selectedItems = listView.ItemsSource?.Cast<Todoitem>().Where(item => item.IsSelected).ToList();

            if (!selectedItems.Any())
            {
                bool Confirmed = await DisplayAlert("Delete All", "Are you sure you want to delete everything?", "OK", "Cancel");

                if (Confirmed)
                {
                    DeleteAllItems(sender, e);
                }
            }
            else
            {
                bool Confirmed = await DisplayAlert("Delete Selected Tasks", "Do you want to delete all selected items?", "Yes", "No");

                if (Confirmed)
                {
                    TodoitemDatabase database = await TodoitemDatabase.Instance;
                    foreach (var item in selectedItems)
                    {
                        await database.DeleteItemAsync(item);
                    }

                    // Refresh ListView
                    await IsEmptyList();
                    await UpdateListView();
                    await deleteSelectedToast.Show(cancellationTokenSource.Token);
                }
            }
        }

        public async void OpenMenu(object sender, EventArgs e)
        {
            string settings = "Settings";

            var action = await Application.Current.MainPage.DisplayActionSheet(null, "Cancel", null, new[] {settings});
            
            if (action != null)
            {
                OpenSettings(sender, e);
            }
            
        }

        //Sorting
        private bool sortByDateAscending = false;

        private void SortByDateClicked(object sender, EventArgs e)
        {
            sortByDateAscending = !sortByDateAscending;

            var sortedItems = sortByDateAscending
                ? ((IEnumerable<Todoitem>)listView.ItemsSource).OrderBy(item => item.Date)
                : ((IEnumerable<Todoitem>)listView.ItemsSource).OrderByDescending(item => item.Date);
            listView.ItemsSource = sortedItems.ToList();
        }

        public async void OpenSortMenu(object sender, EventArgs e)
        {
            string sortbydate = "Most Recent";
            string sortbypriority = "Highest Priority";
            string clearsorting = "Clear Sorting";
            string sortbypinned = "Pinned";

            var action = await Application.Current.MainPage.DisplayActionSheet("Sorting", "Cancel", null, new[] { sortbydate, sortbypriority, sortbypinned, clearsorting });

            if (action != null && action.Equals(sortbydate))
            {
                var sortedItems = ((IEnumerable<Todoitem>)listView.ItemsSource).OrderByDescending(item => item.Date);
                listView.ItemsSource = sortedItems.ToList();
            }
            else if (action != null && action.Equals(sortbypriority))
            {
                var sortedItems = ((IEnumerable<Todoitem>)listView.ItemsSource).OrderBy(item => TodoListPage.GetPriorityValue(item.Priority));
                listView.ItemsSource = sortedItems.ToList();
            }
            else if (action != null && action.Equals(clearsorting)) // Handle clear sorting option
            {
                // Reset ListView
                listView.ItemsSource = ((IEnumerable<Todoitem>)listView.ItemsSource).ToList();
                await UpdateListView();
            }
            else if (action != null && action.Equals(sortbypinned))
            {
                var sortedItems = ((IEnumerable<Todoitem>)listView.ItemsSource).OrderByDescending(item => item.IsPinned);
                listView.ItemsSource = sortedItems.ToList();
            }
        }

        private static int GetPriorityValue(string priority)
        {
            return priority switch
            {
                "Critical" => 1,
                "High" => 2,
                "Medium" => 3,
                "Low" => 4,
                _ => 5 // Default || unknown priorities
            };
        }

        //Searching
        async void SearchBar_TextChangedAsync(object sender, TextChangedEventArgs e)
        {
            var keyword = SearchBar.Text.ToLower();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                // Empty searchbar > show all
                listView.ItemsSource = ((IEnumerable<Todoitem>)listView.ItemsSource).ToList();
                await UpdateListView();
            }
            else
            {
                // Filter items based on the keyword
                var filteredItems = ((IEnumerable<Todoitem>)listView.ItemsSource)
                    .Where(item => item.Name.ToLower().Contains(keyword));

                // Update ListView with filtered items
                listView.ItemsSource = filteredItems.ToList();
            }
        }

        async void SetSelectedItemStatus(object sender, EventArgs e)
        {
            TodoitemDatabase database = await TodoitemDatabase.Instance;
            var selectedItems = listView.ItemsSource?.Cast<Todoitem>().Where(item => item.IsSelected).ToList();
            bool selectedItemsDone = selectedItems.Any(item => item.Done == true);
            bool selectedItemsNotDone = selectedItems.Any(item => item.Done == false);

            if (!selectedItems.Any())
            {
                await DisplayAlert("No Items Selected", "Please select items to mark as complete", "OK");
                return;
            }

            string alertTitleForDone = "Mark selected tasks as complete";
            string alertMessageForDone = "Do you want to mark all selected items as complete?";
            string alertTitleForNotDone = "Mark selected tasks as incomplete";
            string alertMessageForNotDone = "Do you want to mark all selected items as incomplete?";

            //Mark as done
            if (selectedItemsDone)
            {
                bool confirmed = await DisplayAlert(alertTitleForNotDone, alertMessageForNotDone, "Yes", "No");
                if (confirmed)
                {
                    foreach (var item in selectedItems)
                    {
                        item.Done = false;
                        item.IsSelected = false;
                        await database.SaveItemAsync(item);
                    }
                    await UpdateListView();
                }
            }

            //Mark as incomplete
            if (selectedItemsNotDone)
            {
                bool confirmed2 = await DisplayAlert(alertTitleForDone, alertMessageForDone, "Yes", "No");
                if (confirmed2)
                {
                    foreach (var item in selectedItems)
                    {
                        item.Done = true;
                        item.IsSelected = false;
                        await database.SaveItemAsync(item);
                    }
                    await UpdateListView();
                }
            }
        }

        async void SetItemPinned(object sender, EventArgs e)
        {
            TodoitemDatabase database = await TodoitemDatabase.Instance;
            var selectedItems = listView.ItemsSource?.Cast<Todoitem>().Where(item => item.IsSelected).ToList();
            bool selectedItemsPinned = selectedItems.Any(item => item.IsPinned == true);
            bool selectedItemsUnpinned = selectedItems.Any(item => item.IsPinned == false);

            if (!selectedItems.Any())
            {
                await DisplayAlert("No Items Selected", "Please select items to pin", "OK");
                return;
            }

            string alertTitleForPinned = "Pin items";
            string alertMessageForPinned = "Do you want to pin all selected items?";
            string alertTitleForUnpinned = "Unpin items";
            string alertMessageForUnpinned = "Do you want to unpin all selected items?";

            //Unpin
            if (selectedItemsPinned)
            {
                bool confirmed = await DisplayAlert(alertTitleForUnpinned, alertMessageForUnpinned, "Yes", "No");
                if (confirmed)
                {
                    foreach (var item in selectedItems)
                    {
                        item.IsPinned = false;
                        item.IsSelected = false;
                        await database.SaveItemAsync(item);
                    }
                    await UpdateListView();
                    await UpdateCollectionView();
                }
            }

            //Pin
            if (selectedItemsUnpinned)
            {
                bool confirmed2 = await DisplayAlert(alertTitleForPinned, alertMessageForPinned, "Yes", "No");
                if (confirmed2)
                {
                    foreach (var item in selectedItems)
                    {
                        item.IsPinned = true;
                        item.IsSelected = false;
                        await database.SaveItemAsync(item);
                    }
                    await UpdateListView();
                    await UpdateCollectionView();
                }
            }
        }

        async void SetSelectedItemPriority(object sender, EventArgs e)
        {
            var selectedItems = listView.ItemsSource?.Cast<Todoitem>().Where(item => item.IsSelected).ToList();

            if(!selectedItems.Any())
            {
                await DisplayAlert("No Items Selected", "Please select items to set priority", "OK");
            }
            else
            {
                var action = await Application.Current.MainPage.DisplayActionSheet("Set Priority", "Cancel", null, new[] { "Low", "Medium", "High", "Critical" });

                if (action != null)
                {
                    foreach (var item in selectedItems)
                    {
                        item.Priority = action;
                        item.IsSelected = false; // Uncheck
                    }

                    TodoitemDatabase database = await TodoitemDatabase.Instance;
                    foreach (var item in selectedItems)
                    {
                        await database.SaveItemAsync(item);
                    }
                    await UpdateListView();
                }
            }
        }

        private async Task SetPinnedOnlyListSource()
        {
            TodoitemDatabase database = await TodoitemDatabase.Instance;
            var pinnedItems = await database.GetItemsPinnedAysnc();
            pinnedList.ItemsSource = pinnedItems;
        }

        private async void RefreshView_Refreshing(object sender, EventArgs e)
        {
            listView.IsRefreshing = true;
            await UpdateListView();
            await Task.Delay(1000); // Temporary fix, sometimes the refresh spinner doesn't disappear.
            listView.IsRefreshing = false;
        }

        private async void ListView_Scrolled(object sender, ScrolledEventArgs e)
        {
            await SearchBar.HideKeyboardAsync();

            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                var scrollThreshold = 70;
                if (e.ScrollY > scrollThreshold)
                {
                    // Scroll down
                    if (pinnedcontainer.IsVisible)
                    {
                        await pinnedcontainer.FadeTo(0, 250);
                        pinnedcontainer.IsVisible = false;
                    }
                }
                else
                {
                    // Scroll up
                    if (!pinnedcontainer.IsVisible)
                    {
                        pinnedcontainer.IsVisible = true;
                        await pinnedcontainer.FadeTo(1, 250);
                    }
                }
            }
        }
    }
}