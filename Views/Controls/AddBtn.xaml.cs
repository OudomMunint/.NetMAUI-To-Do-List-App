namespace ToDoListApp.Views.Controls
{
    public partial class AddBtn
    {
        private double _initialX;
        private double _initialY;

        public AddBtn()
        {
            InitializeComponent();
        }

        private void DragGestureRecognizer_DragStarting(object sender, DragStartingEventArgs e)
        {
            if (sender is View view)
            {
                _initialX = view.TranslationX;
                _initialY = view.TranslationY;

                // Add position data to payload
                e.Data.Properties.Add("InitialX", _initialX);
                e.Data.Properties.Add("InitialY", _initialY);
            }
        }

        private void DragGestureRecognizer_DropCompleted(object sender, DropCompletedEventArgs e)
        {
            if (sender is View view)
            {
                // Update the final position based on the initial position
                // DropCompletedEventArgs doesn't provide position info, so we use the stored initial values
                view.TranslationX = _initialX;
                view.TranslationY = _initialY;
            }
        }
    }
}