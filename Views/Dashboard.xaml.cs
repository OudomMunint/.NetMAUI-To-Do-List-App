using Microcharts;
using Microcharts.Maui;
using SkiaSharp;
using ToDoListApp.Data;
using ToDoListApp.Models;
using ToDoListApp.Views;
using System;
using System.Linq;
using System.Threading.Tasks;

#if IOS
using UIKit;
#endif

namespace ToDoListApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Dashboard : ContentPage
    {
        private int totalItems;
        private int doneItems;
        private int notDone;
        private ChartEntry[] entries;
        private int itemsWithAttachment;
        private int itemsWoAttachment;

        public string ItemHasAttachment { get; set; }

        public Dashboard()
        {
            InitializeComponent();

#if IOS
            UINavigationController vc = (UINavigationController)Platform.GetCurrentUIViewController();//using UIKit, find the UINavigationController  
            vc.InteractivePopGestureRecognizer.Enabled = false;
#endif
        }

        protected override bool OnBackButtonPressed()
        {
            // Return true to prevent back button 
            return true;
        }

        private async Task CountItemsHasAttachment()
        {
            TodoitemDatabase database = await TodoitemDatabase.Instance;
            var todoItem = (Todoitem)BindingContext;
            ItemHasAttachment = ((IEnumerable<Todoitem>)listView.ItemsSource).Count(item => item.HasAttachment == true).ToString();
            itemsWithAttachment = ((IEnumerable<Todoitem>)listView.ItemsSource).Count(item => item.HasAttachment == true);
            itemsWoAttachment = ((IEnumerable<Todoitem>)listView.ItemsSource).Count(item => item.HasAttachment == false);

            //hasattcount.Text = $"📎 {ItemHasAttachment} Has Attachments";
        }

        private async Task UpdateListView()
        {
            TodoitemDatabase database = await TodoitemDatabase.Instance;
            listView.ItemsSource = await database.GetItemsAysnc();
        }

        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                await UpdateListView();
                await CountItemsHasAttachment();
                await Task.WhenAll(
                    GetTotalItems(),
                    GetDoneItems(),
                    UpdateLabel()
                );
                CreateChart1();
                CreateChart2();
                CreateChart3();
            }
            catch (Exception ex)
            {
                // Handle the exception here
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        // Dispose both charts, fixes #155
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            chartView.Chart = null;
            chartView2.Chart = null;
            chartView3.Chart = null;
        }

        private Task UpdateLabel()
        {
            int pinnedItems = ((IEnumerable<Todoitem>)listView.ItemsSource).Count(item => item.IsPinned);
            int totalTodoItems = listView.ItemsSource?.Cast<object>().Count() ?? 0;
            todoitems.Text = $"📋 {totalItems} Total";
            todoitems2.Text = $"📌 {pinnedItems} Pinned";

            int lowPriorityItems = ((IEnumerable<Todoitem>)listView.ItemsSource).Count(item => item.Priority == "Low");
            lowpriority.Text = $"🟢 {lowPriorityItems} Low";

            int mediumPriorityItems = ((IEnumerable<Todoitem>)listView.ItemsSource).Count(item => item.Priority == "Medium");
            mediumpriority.Text = $"🟡 {mediumPriorityItems} Medium";

            int highPriorityItems = ((IEnumerable<Todoitem>)listView.ItemsSource).Count(item => item.Priority == "High");
            highpriority.Text = $"🟠 {highPriorityItems} High";

            int criticalPriorityItems = ((IEnumerable<Todoitem>)listView.ItemsSource).Count(item => item.Priority == "Critical");
            criticalpriority.Text = $"🟤 {criticalPriorityItems} Critical";

            return Task.CompletedTask;
        }

        private Task GetTotalItems()
        {
            totalItems = listView.ItemsSource?.Cast<object>().Count() ?? 0;
            return Task.CompletedTask;
        }

        private Task GetDoneItems()
        {
            doneItems = ((IEnumerable<Todoitem>)listView.ItemsSource).Count(item => item.Done);
            notDone = totalItems - doneItems;
            return Task.CompletedTask;
        }

        void TapGestureRecognizer_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
        {
            Navigation.PushAsync(new TodoListPage());
        }

        private void CreateChart1()
        {
            int lowPriorityItems = ((IEnumerable<Todoitem>)listView.ItemsSource).Count(item => item.Priority == "Low");
            int mediumPriorityItems = ((IEnumerable<Todoitem>)listView.ItemsSource).Count(item => item.Priority == "Medium");
            int highPriorityItems = ((IEnumerable<Todoitem>)listView.ItemsSource).Count(item => item.Priority == "High");
            int criticalPriorityItems = ((IEnumerable<Todoitem>)listView.ItemsSource).Count(item => item.Priority == "Critical");

            entries = new[]
            {
                new ChartEntry(criticalPriorityItems)
                {
                    Label = "Critical",
                    //ValueLabel = criticalPriorityItems.ToString(),
                    Color = SKColor.Parse("#808080")
                },

                new ChartEntry(lowPriorityItems)
                {
                    Label = "Low",
                    //ValueLabel = lowPriorityItems.ToString(),
                    Color = SKColor.Parse("#00ff00")
                },

                new ChartEntry(mediumPriorityItems)
                {
                    Label = "Medium",
                    //ValueLabel = mediumPriorityItems.ToString(),
                    Color = SKColor.Parse("#D5B60A")
                },

                new ChartEntry(highPriorityItems)
                {
                    Label = "High",
                    //ValueLabel = highPriorityItems.ToString(),
                    Color = SKColor.Parse("#FFA500")
                },
            };

            // Check if there are any items in the list
            if (entries.Any(entry => entry.Value > 0))
            {
                chartView.Chart = new RadarChart
                {
                    Entries = entries,
                    IsAnimated = false,
                    BackgroundColor = SKColor.Parse("#00FFFFFF"),
#if ANDROID
                    Margin = 65
#endif
                };
            }
            else
            {
                // Create an empty chart with a placeholder entry
                entries = new[]
                {
                    new ChartEntry(1)
                    {
                        Label = "",
                        ValueLabel = "0",
                        Color = SKColor.Parse("#CCCCCC")
                    }
                };

                chartView.Chart = new PieChart
                {
                    Entries = entries,
                    IsAnimated = true,
                    BackgroundColor = SKColor.Parse("#00FFFFFF"),
                    LabelMode = LabelMode.None,
#if ANDROID
                    Margin = 30
#endif
#if IOS
                    Margin = 0
#endif
                };
            }
        }

        private void CreateChart2()
        {
            AppTheme darkmode = AppTheme.Dark;

            // Define colors for dark mode and light mode
            SKColor darkModeColor = SKColor.Parse("#f7f9fa");
            SKColor lightModeColor = SKColor.Parse("#070808");

            // Determine which color to use based on the current theme
            SKColor labelColor = SKColor.Parse("#FFFFFF");

            if (Application.Current.RequestedTheme == darkmode)
            {
                labelColor = darkModeColor;
            }
            else
            {
                labelColor = lightModeColor;
            }

            // Check if there are any items in the list
            if (doneItems + notDone > 0)
            {
                entries = new[]
                {
                    new ChartEntry(doneItems)
                    {
                        Label = "Completed",
                        ValueLabel = doneItems.ToString(),
                        Color = SKColor.Parse("#2c3e50"),
                        ValueLabelColor = labelColor
                    },

                    new ChartEntry(notDone)
                    {
                        Label = "Opened",
                        ValueLabel = notDone.ToString(),
                        Color = SKColor.Parse("#ADD8E6"),
                        ValueLabelColor = labelColor
                    }
                };

                chartView2.Chart = new DonutChart
                {
                    Entries = entries,
                    IsAnimated = true,
                    BackgroundColor = SKColor.Parse("#00FFFFFF"),
                    LabelTextSize = 30,
                    LabelColor = labelColor,
                    GraphPosition = GraphPosition.Center
                };
            }
            else
            {
                // Create an empty chart with a placeholder entry
                entries = new[]
                {
                    new ChartEntry(1)
                    {
                        Label = "Completed Tasks",
                        ValueLabel = "0",
                        Color = SKColor.Parse("#CCCCCC"),
                        ValueLabelColor = labelColor
                    },

                    new ChartEntry(1)
                    {
                        Label = "Opened Tasks",
                        ValueLabel = "0",
                        Color = SKColor.Parse("#CCCCCC"),
                        ValueLabelColor = labelColor
                    }
                };

                chartView2.Chart = new DonutChart
                {
                    Entries = entries,
                    IsAnimated = true,
                    BackgroundColor = SKColor.Parse("#00FFFFFF"),
                    LabelTextSize = 30,
                    LabelColor = labelColor,
                    GraphPosition = GraphPosition.Center
                };
            }
        }

        private void CreateChart3()
        {
            AppTheme darkmode = AppTheme.Dark;

            // Define colors for dark mode and light mode
            SKColor darkModeColor = SKColor.Parse("#f7f9fa");
            SKColor lightModeColor = SKColor.Parse("#070808");

            // Determine which color to use based on the current theme
            SKColor labelColor = SKColor.Parse("#FFFFFF");

            if (Application.Current.RequestedTheme == darkmode)
            {
                labelColor = darkModeColor;
            }
            else
            {
                labelColor = lightModeColor;
            }

            // Check if there are any items in the list
            if (itemsWithAttachment + itemsWoAttachment > 0)
            {
                entries = new[]
                {
                    new ChartEntry(itemsWithAttachment)
                    {
                        Label = "Has Attachment",
                        ValueLabel = itemsWithAttachment.ToString(),
                        Color = SKColor.Parse("#ADD8E6"),
                        ValueLabelColor = labelColor,
                    },

                    new ChartEntry(itemsWoAttachment)
                    {
                        Label = "No Attachment",
                        ValueLabel = itemsWoAttachment.ToString(),
                        Color = SKColor.Parse("#2c3e50"),
                        ValueLabelColor = labelColor,
                    }
                };

                chartView3.Chart = new PieChart
                {
                    Entries = entries,
                    IsAnimated = true,
                    BackgroundColor = SKColor.Parse("#00FFFFFF"),
                    LabelTextSize = 30,
                    LabelColor = labelColor,
                    LabelMode = LabelMode.LeftAndRight,
                    GraphPosition = GraphPosition.Center
                };
            }
            else
            {
                // Create an empty chart with a placeholder entry
                entries = new[]
                {
                    new ChartEntry(1)
                    {
                        Label = "Has Attachment",
                        ValueLabel = "0",
                        Color = SKColor.Parse("#CCCCCC"),
                        ValueLabelColor = labelColor,
                    },

                    new ChartEntry(1)
                    {
                        Label = "No Attachment",
                        ValueLabel = "0",
                        Color = SKColor.Parse("#CCCCCC"),
                        ValueLabelColor = labelColor,

                    }
                };

                chartView3.Chart = new PieChart 
                {
                    Entries = entries,
                    IsAnimated = true,
                    BackgroundColor = SKColor.Parse("#00FFFFFF"),
                    LabelTextSize = 30,
                    LabelColor = labelColor,
                    LabelMode = LabelMode.LeftAndRight,
                    GraphPosition = GraphPosition.Center
                };
            }
        }
    }
}