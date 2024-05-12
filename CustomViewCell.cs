using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace ToDoListApp;

public class CustomViewCell : Microsoft.Maui.Controls.ViewCell
{
    public static readonly BindableProperty SelectedBackgroundColorProperty = BindableProperty.Create(nameof(SelectedBackgroundColor), typeof(Color), typeof(CustomViewCell), Colors.White);

    public Color SelectedBackgroundColor
    {
        get { return (Color)GetValue(SelectedBackgroundColorProperty); }
        set { SetValue(SelectedBackgroundColorProperty, value); }
    }

    public CustomViewCell()
    {

    }
}