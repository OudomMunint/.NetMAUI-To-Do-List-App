using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListApp.Data;
using ToDoListApp.Models;

namespace ToDoListApp.Views
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TodoitemPage : ContentPage
	{
        public TodoitemPage()
		{
			InitializeComponent();
		}

        async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameField.Text) || string.IsNullOrWhiteSpace(DescField.Text))
            {
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
            else
            {
                todoItem.Priority = "Critical";
            }

            TodoitemDatabase database = await TodoitemDatabase.Instance;
            await database.SaveItemAsync(todoItem);
            await Navigation.PopAsync();
        }

        async void OnDeleteClicked(object sender, EventArgs e)
		{
			var todoItem = (Todoitem)BindingContext;
			TodoitemDatabase database = await TodoitemDatabase.Instance;
			await database.DeleteItemAsync(todoItem);
			await Navigation.PopAsync();
		}

		async void OnCancelClicked(object sender, EventArgs e)
		{
			await Navigation.PopAsync();
		}

        private void OnClearClicked(object sender, EventArgs e)
        {
            NameField.Text = " ";
            DescField.Text = " ";
        }
    }
}