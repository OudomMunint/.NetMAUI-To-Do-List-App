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
			var todoItem = (Todoitem)BindingContext;
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
	}
}