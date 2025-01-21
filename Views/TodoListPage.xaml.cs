using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using ToDoListApp.Data;
using ToDoListApp.Models;
using CommunityToolkit.Maui.Core.Platform;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using static ToDoListApp.ToastService;
using System;
using System.ComponentModel;

namespace ToDoListApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TodoListPage : ContentPage, INotifyPropertyChanged
    {
        private readonly AppTheme darkmode = AppTheme.Dark;

        public ListView todolistlist;

        private bool sortByDateAscending = false;

        private bool isPageLoading;

        public bool IsPageLoading
        {
            get => isPageLoading;
            set
            {
                if (isPageLoading != value)
                {
                    isPageLoading = value;
                    OnPropertyChanged(nameof(IsPageLoading));
                }
            }
        }

        public TodoListPage()
        {
            InitializeComponent();

            BindingContext = this;
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

        protected override async void OnAppearing()
        {
            try
            {
#if ANDROID
                IsPageLoading = true;
#endif

                base.OnAppearing();
                await IsEmptyList();
                await GetItemsWithAttachment();
                await SetPinnedOnlyListSource();
                await UpdateListView();
                await UpdateCollectionView();
                ApplySavedSorting();

#if ANDROID
                IsPageLoading = false;
#endif
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "Cancel");
                Console.WriteLine(ex);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
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

        async void DeleteAllItems(object sender, EventArgs e)
        {
            bool userConfirmed = await DisplayAlert("Delete All Tasks", "Confirm you want to DELETE ALL items?", "Yes", "No");

            if (userConfirmed)
            {
                try
                {
                    TodoitemDatabase database = await TodoitemDatabase.Instance;
                    var allitems = await database.GetItemsAysnc();

                    foreach (var item in allitems)
                    {
                        await database.DeleteItemAsync(item);
                    }

                    //listView.ItemsSource = null;
                    await IsEmptyList();
                    await UpdateListView();
                    HapticFeedback.Perform(HapticFeedbackType.Click);
                    await ShowToastAsync("All Task(s) Deleted 🗑", 16, ToastDuration.Short);
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
            try
            {
                Routing.RegisterRoute(nameof(TodoitemPage), typeof(TodoitemPage));

                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
                if (e.SelectedItem != null)
                {
                    await Navigation.PushAsync(new TodoitemPage
                    {
                        BindingContext = e.SelectedItem as Todoitem
                    });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "OK");
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
            //labeltitle.Text = $"🏠 {totalItems} Opened";
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
                    await ShowToastAsync("Selected Task(s) Deleted 🗑️", 16, ToastDuration.Short);
                }
            }
        }

        private void SaveSortingPreference(string sortingType, bool isAscending)
        {
            Preferences.Set("SortingKey", sortingType);
            Preferences.Set("SortDirectionKey", isAscending);
        }

        private void ApplySavedSorting()
        {
            var sortingType = Preferences.Get("SortingKey", "none");
            var isAscending = Preferences.Get("SortDirectionKey", true);

            IEnumerable<Todoitem> items = listView.ItemsSource as IEnumerable<Todoitem>;

            switch (sortingType)
            {
                case "date":
                    items = isAscending ? items.OrderBy(item => item.Date) : items.OrderByDescending(item => item.Date);
                    break;
                case "priority":
                    items = items.OrderBy(item => TodoListPage.GetPriorityValue(item.Priority));
                    break;
                case "pinned":
                    items = items.OrderByDescending(item => item.IsPinned);
                    break;
                case "none":
                default:
                    return; // No sorting
            }

            listView.ItemsSource = items.ToList();
        }

        //Sorting
        private void SortByDateClicked(object sender, EventArgs e)
        {
            sortByDateAscending = !sortByDateAscending;

            var sortedItems = sortByDateAscending
                ? ((IEnumerable<Todoitem>)listView.ItemsSource).OrderBy(item => item.Date)
                : ((IEnumerable<Todoitem>)listView.ItemsSource).OrderByDescending(item => item.Date);
            listView.ItemsSource = sortedItems.ToList();

            SaveSortingPreference("date", sortByDateAscending);
        }

        public async void OpenSortMenu(object sender, EventArgs e)
        {
            string sortbydate = "Most Recent";
            string sortbypriority = "Highest Priority";
            string clearsorting = "Clear Sorting";
            string sortbypinned = "Pinned";

            var action = await Application.Current.MainPage.DisplayActionSheet("Sorting", "Cancel", null, new[] { sortbydate, sortbypriority, sortbypinned, clearsorting });

            if (action == sortbydate)
            {
                var sortedItems = ((IEnumerable<Todoitem>)listView.ItemsSource).OrderByDescending(item => item.Date);
                listView.ItemsSource = sortedItems.ToList();
                SaveSortingPreference("date", false); // Descending
            }
            else if (action == sortbypriority)
            {
                var sortedItems = ((IEnumerable<Todoitem>)listView.ItemsSource).OrderBy(item => TodoListPage.GetPriorityValue(item.Priority));
                listView.ItemsSource = sortedItems.ToList();
                SaveSortingPreference("priority", true); // Ascending
            }
            else if (action == sortbypinned)
            {
                var sortedItems = ((IEnumerable<Todoitem>)listView.ItemsSource).OrderByDescending(item => item.IsPinned);
                listView.ItemsSource = sortedItems.ToList();
                SaveSortingPreference("pinned", false); // Descending
            }
            else if (action == clearsorting)
            {
                listView.ItemsSource = ((IEnumerable<Todoitem>)listView.ItemsSource).ToList();
                Preferences.Remove("SortingKey");
                Preferences.Remove("SortDirectionKey");
                await UpdateListView();
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

            if (!selectedItems.Any())
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
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    listView.IsRefreshing = true;
                    listView.ItemsSource = null; // Work around for blank rows issue
                });

                await UpdateListView();
                await Task.Delay(1000); // Temporary fix, sometimes the refresh spinner doesn't disappear.
                listView.IsRefreshing = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "OK");
            }
        }

        async void listView_Scrolled2(System.Object sender, Microsoft.Maui.Controls.ScrolledEventArgs e)
        {
            var scrollThreshold = 70;
            var scrollThreshold2 = 1;

            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                await SearchBar.HideKeyboardAsync();
            }

            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {

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

            if (DeviceInfo.Platform == DevicePlatform.Android && e.ScrollY > scrollThreshold2)
            {
                await SearchBar.HideKeyboardAsync();
                SearchBar.Unfocus();
            }
        }

        private async void pinitem_Clicked(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                HapticFeedback.Perform(HapticFeedbackType.Click);
                var todoItem = menuItem.BindingContext as Todoitem;
                if (todoItem != null)
                {
                    todoItem.IsPinned = true;

                    TodoitemDatabase database = await TodoitemDatabase.Instance;
                    await database.SaveItemAsync(todoItem);

                    await UpdateListView();
                    await UpdateCollectionView();

                    await ShowToastAsync(todoItem.Name + " Pinned 📌", 16, ToastDuration.Long);
                }
            }
        }

        private async void deleteitem_Clicked(object sender, EventArgs e)
        {
            HapticFeedback.Perform(HapticFeedbackType.Click);
            var menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                var todoItem = menuItem.BindingContext as Todoitem;
                if (todoItem != null)
                {
                    TodoitemDatabase database = await TodoitemDatabase.Instance;

                    await database.DeleteItemAsync(todoItem);
                    await database.SaveItemAsync(todoItem);

                    await UpdateListView();
                    await UpdateCollectionView();

                    await ShowToastAsync(todoItem.Name + " Deleted 🗑️", 16, ToastDuration.Long);
                }
            }
        }

        private async void markasdoneitem_Clicked(System.Object sender, System.EventArgs e)
        {
            HapticFeedback.Perform(HapticFeedbackType.Click);
            var menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                var todoItem = menuItem.BindingContext as Todoitem;
                if (todoItem != null)
                {
                    if (todoItem.Done == true)
                    {
                        await DisplayAlert("Task already completed", null, "OK");
                    }

                    else
                    {
                        todoItem.Done = true;

                        TodoitemDatabase database = await TodoitemDatabase.Instance;
                        await database.SaveItemAsync(todoItem);

                        await UpdateListView();
                        await ShowToastAsync(todoItem.Name + " marked as complete ✅", 16, ToastDuration.Long);
                    }
                }
            }
        }
    }
}