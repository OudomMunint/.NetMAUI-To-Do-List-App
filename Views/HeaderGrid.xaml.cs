using Microsoft.Maui.Controls;
using System;

namespace ToDoListApp.Views
{
    public partial class HeaderGrid : ContentView
    {
        private readonly TimeSpan updateInterval = TimeSpan.FromMinutes(1);
        private readonly System.Threading.Timer timer;
        public string greeting = "Hello 👋";

        public HeaderGrid()
        {
            InitializeComponent();
            BindingContext = this;

            // Update every minute
            timer = new System.Threading.Timer(UpdateCurrentDate, null, TimeSpan.Zero, updateInterval);
            
            // Set initial value
            UpdateCurrentDate(null);

            titleLabel.Text = CurrentDate + ", " + greeting;
        }

        private string _currentDate;
        public string CurrentDate 
        { 
            get => _currentDate; 
            set
            {
                if (_currentDate != value)
                {
                    _currentDate = value;
                    OnPropertyChanged(nameof(CurrentDate));
                }
            }
        }

        private void UpdateCurrentDate(object state)
        {
            CurrentDate = DateTime.Now.ToString($"🗓️ MMMM dd, yyyy");
        }
    }
}