using Microsoft.Maui.Controls;
using ToDoListApp.Data;
using ToDoListApp.Models;

namespace ToDoListApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TodoListPage : ContentPage
    {
        public TodoListPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await UpdateListView();
        }

        private async Task UpdateListView()
        {
            TodoitemDatabase database = await TodoitemDatabase.Instance;
            listView.ItemsSource = await database.GetItemsAysnc();
            UpdateTitle();
        }

        async void OnItemAdded(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TodoitemPage
            {
                BindingContext = new Todoitem()
            });
        }

        public async void OpenMenu(object sender, EventArgs e)
        {
            string deleteall = "Delete all";
            string settings = "Settings";

            var action = await Application.Current.MainPage.DisplayActionSheet(null, "Cancel", null, new[] { deleteall, settings });

            if (action != null && action.Equals(deleteall))
            {
                DeleteAllItems(sender, e);
            }
            else if (action != null && action.Equals(settings))
            {
                OpenSettings(sender, e);
            }
        }

        async void OpenSettings(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Settings());
        }

        async void DeleteAllItems(object sender, EventArgs e)
        {
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
                await UpdateListView();
            }
        }

        async void OnDeleteClicked(object sender, EventArgs e)
        {
            var todoItem = (Todoitem)BindingContext;
            TodoitemDatabase database = await TodoitemDatabase.Instance;
            await database.DeleteItemAsync(todoItem);
            await UpdateListView(); // Update the ListView after deleting an item.
            await Navigation.PopAsync();
        }

        async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
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
            Title = $"🏠🗒️ {totalItems} Opened";
            // increase the font size of the title
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
                await DisplayAlert("No Items Selected", "Please select items to delete", "OK");
            }
            else
            {
                bool Confirmed = await DisplayAlert("Delete Selected Tasks", "Confirm you want to delete selected items?", "Yes", "No");

                if (Confirmed)
                {
                    TodoitemDatabase database = await TodoitemDatabase.Instance;
                    foreach (var item in selectedItems)
                    {
                        await database.DeleteItemAsync(item);
                    }

                    // Refresh ListView 
                    await UpdateListView();
                }
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
            //await UpdateListView();
        }

        //Searching
        async void SearchBar_TextChangedAsync(object sender, TextChangedEventArgs e)
        {
            var keyword = SearchBar.Text.ToLower();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                // If the search bar is empty, reset the ListView to display all items
                listView.ItemsSource = ((IEnumerable<Todoitem>)listView.ItemsSource).ToList();
                await UpdateListView();
            }
            else
            {
                // Filter items based on the search bar keyword
                var filteredItems = ((IEnumerable<Todoitem>)listView.ItemsSource)
                
                    .Where(item => item.Name.ToLower().Contains(keyword));

                // Update the ListView with filtered items
                listView.ItemsSource = filteredItems.ToList();
            }
        }

        async void MarkSelectedItemsComplete(object sender, EventArgs e)
        {
            var selectedItems = listView.ItemsSource?.Cast<Todoitem>().Where(item => item.IsSelected).ToList();

            if (!selectedItems.Any())
            {
                await DisplayAlert("No Items Selected", "Please select items to mark as complete", "OK");
            }
            else
            {
                foreach (var item in selectedItems)
                {
                    item.Done = true;
                    item.IsSelected = false; // Uncheck
                }

                bool Confirmed = await DisplayAlert("Mark Selected Tasks Complete", "Confirm you want to mark selected items as complete?", "Yes", "No");

                if (Confirmed)
                {
                    TodoitemDatabase database = await TodoitemDatabase.Instance;
                    foreach (var item in selectedItems)
                    {
                        await database.SaveItemAsync(item);
                    }
                    await UpdateListView();
                }
            }
        }
    }
}