using Microcharts;
using Microcharts.Maui;
using SkiaSharp;
using ToDoListApp.Data;
using ToDoListApp.Models;
using ToDoListApp.Views;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoListApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Dashboard : ContentPage
    {
        public enum ChartType { Priority, CompletionStatus, Attachments }
        private int totalItems;
        private int doneItems;
        private int notDone;
        private int itemsWithAttachment;
        private int itemsWoAttachment;

        public Dashboard()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                await CountItemsHasAttachment();
                await Task.WhenAll(GetTotalItems(), GetDoneItems(), GetPinnedAndPriority());
                CreateChart(ChartType.Priority);
                CreateChart(ChartType.CompletionStatus);
                CreateChart(ChartType.Attachments);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        // Dispose all charts, fixes #155
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            new[] { chartView, chartView2, chartView3 }.ToList().ForEach(chart => chart.Chart = null);
        }

        private async Task GetPinnedAndPriority()
        {
            TodoitemDatabase database = await TodoitemDatabase.Instance;

            int pinnedItems = await database.GetPinnedCountAysnc(true);
            todoitems2.Text = $"📌 {pinnedItems} Pinned";

            int lowPriorityItems = await database.GetItemCountByPriority("Low");
            lowpriority.Text = $"🟢 {lowPriorityItems} Low";

            int mediumPriorityItems = await database.GetItemCountByPriority("Medium");
            mediumpriority.Text = $"🟡 {mediumPriorityItems} Medium";

            int highPriorityItems = await database.GetItemCountByPriority("High");
            highpriority.Text = $"🟠 {highPriorityItems} High";

            int criticalPriorityItems = await database.GetItemCountByPriority("Critical");
            criticalpriority.Text = $"🟤 {criticalPriorityItems} Critical";
        }

        private async Task GetTotalItems()
        {
            TodoitemDatabase database = await TodoitemDatabase.Instance;
            totalItems = await database.GetTotalItems();
            todoitems.Text = $"📋 {totalItems} Total";
        }

        private async Task GetDoneItems()
        {
            TodoitemDatabase database = await TodoitemDatabase.Instance;
            var doneItemsList = await database.GetItemsDoneAsync();
            doneItems = doneItemsList.Count;
            notDone = totalItems - doneItems;
        }

        private async Task CountItemsHasAttachment()
        {
            TodoitemDatabase database = await TodoitemDatabase.Instance;
            itemsWithAttachment = await database.GetItemAttachmentStatus(true);
            itemsWoAttachment = await database.GetItemAttachmentStatus(false);
        }

        private async void CreateChart(ChartType chartType)
        {
            TodoitemDatabase database = await TodoitemDatabase.Instance;
            AppTheme darkmode = AppTheme.Dark;
            SKColor labelColor = Application.Current.RequestedTheme == darkmode ? SKColor.Parse("#f7f9fa") : SKColor.Parse("#070808");

            ChartEntry[] entries;
            Chart chart;

            switch (chartType)
            {
                case ChartType.Priority:
                    var priorities = new[]
                    {
                        (Label: "Critical", Count: await database.GetItemCountByPriority("Critical"), Color: "#808080"),
                        (Label: "Low", Count: await database.GetItemCountByPriority("Low"), Color: "#00ff00"),
                        (Label: "Medium", Count: await database.GetItemCountByPriority("Medium"), Color: "#D5B60A"),
                        (Label: "High", Count: await database.GetItemCountByPriority("High"), Color: "#FFA500")
                    };

                    if (priorities.Any(p => p.Count > 0))
                    {
                        entries = priorities.Select(p => new ChartEntry(p.Count)
                        {
                            Label = p.Label,
                            Color = SKColor.Parse(p.Color)
                        }).ToArray();

                        chart = new RadarChart
                        {
                            Entries = entries,
                            IsAnimated = false,
                            BackgroundColor = SKColor.Parse("#00FFFFFF"),
#if ANDROID
                            Margin = 60
#endif
                        };
                    }
                    else
                    {
                        chart = CreatePlaceholderChart(labelColor, typeof(RadarChart));
                    }
                    chartView.Chart = chart;
                    break;

                case ChartType.CompletionStatus:
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
                        chart = new DonutChart
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
                        chart = CreatePlaceholderChart(labelColor, typeof(DonutChart));
                    }
                    chartView2.Chart = chart;
                    break;

                case ChartType.Attachments:
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
                        chart = new PieChart
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
                        chart = CreatePlaceholderChart(labelColor, typeof(PieChart));
                    }
                    chartView3.Chart = chart;
                    break;
            }
        }

        private Chart CreatePlaceholderChart(SKColor labelColor, Type chartType)
        {
            ChartEntry[] entries;

            if (chartType == typeof(DonutChart))
            {
                entries = new[]
                {
                    new ChartEntry(1) { Label = "Completed Tasks", ValueLabel = "0", Color = SKColor.Parse("#CCCCCC"), ValueLabelColor = labelColor },
                    new ChartEntry(1) { Label = "Opened Tasks", ValueLabel = "0", Color = SKColor.Parse("#CCCCCC"), ValueLabelColor = labelColor }
                };

                return new DonutChart
                {
                    Entries = entries,
                    IsAnimated = true,
                    BackgroundColor = SKColor.Parse("#00FFFFFF"),
                    LabelTextSize = 30,
                    LabelColor = labelColor,
                    GraphPosition = GraphPosition.Center
                };
            }

            if (chartType == typeof(PieChart))
            {
                entries = new[]
                    {
                        new ChartEntry(1) { Label = "Has Attachment", ValueLabel = "0", Color = SKColor.Parse("#CCCCCC"), ValueLabelColor = labelColor },
                        new ChartEntry(1) { Label = "No Attachment", ValueLabel = "0", Color = SKColor.Parse("#CCCCCC"), ValueLabelColor = labelColor }
                    };

                return new PieChart
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

            if (chartType == typeof(RadarChart))
            {
                entries = new[]
                {
                    new ChartEntry(1) { Label = "L", Color = SKColor.Parse("#00FFFFFF"), ValueLabelColor = labelColor },
                    new ChartEntry(1) { Label = "M", Color = SKColor.Parse("#00FFFFFF"), ValueLabelColor = labelColor },
                    new ChartEntry(1) { Label = "H", Color = SKColor.Parse("#00FFFFFF"), ValueLabelColor = labelColor },
                    new ChartEntry(1) { Label = "C", Color = SKColor.Parse("#00FFFFFF"), ValueLabelColor = labelColor }
                };

                return new RadarChart
                {
                    Entries = entries,
                    IsAnimated = true,
                    BackgroundColor = SKColor.Parse("#00FFFFFF"),
                    LabelTextSize = 30,
                    LabelColor = labelColor,
                };
            }

            throw new ArgumentException("Unsupported chart type", nameof(chartType));
        }
    }
}