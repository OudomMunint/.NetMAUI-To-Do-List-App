using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Platform;

#if ANDROID
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
#endif

namespace ToDoListApp;

internal class AndroidHandlers
{
    internal static void Apply()
    {
#if ANDROID
        // Editor Handler
        Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("StyledEditorCustomization", (h, v) =>
        {
            // Remove underline:
            h.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToAndroid());
            h.PlatformView.SetBackgroundColor(Colors.WhiteSmoke.ToAndroid());
            h.PlatformView.SetPadding(25, 5, 25, 5);
        });

        // Entry Handler
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("StyleEntryCustomization", (h, v) =>
        {
            // Customize Entry objects of type StyleEntry
                // Remove underline:
                h.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToAndroid());

                // Add rounded corner border
                var stroke = new SolidColorBrush(Colors.DarkGray);
                var strokeShape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(3, 3, 3, 3)
                };
                var border = new Border
                {
                    Stroke = stroke,
                    StrokeThickness = 1,
                    StrokeShape = strokeShape
                };
                h.PlatformView.UpdateBorderStroke(border);
                h.PlatformView.SetPadding(25, 5, 25, 5);

                // Set tint color
                h.PlatformView.TextCursorDrawable.SetTint(Colors.DarkGray.ToPlatform());
        });

        // Date Handler
        Microsoft.Maui.Handlers.DatePickerHandler.Mapper.AppendToMapping("DatePickerCustomization", (h, v) =>
        {
            // Remove underline:
            h.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToAndroid());
            h.PlatformView.SetBackgroundColor(Colors.WhiteSmoke.ToAndroid());
            h.PlatformView.SetPadding(25, 5, 25, 5);
        });

        Microsoft.Maui.Handlers.SearchBarHandler.Mapper.AppendToMapping("LightSearch", (h, v) =>
        {
            Android.Widget.LinearLayout linearLayout = h.PlatformView.GetChildAt(0) as Android.Widget.LinearLayout;
            linearLayout = linearLayout.GetChildAt(2) as Android.Widget.LinearLayout;
            linearLayout = linearLayout.GetChildAt(1) as Android.Widget.LinearLayout;
            linearLayout.Background = null;

            var children = h.PlatformView.GetChildrenOfType<Android.Widget.ImageView>();
            foreach (var child in children)
            {
                child.SetColorFilter(Colors.DarkGray.ToPlatform());
            }

        });

        // remove underline from Picker
        Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("PickerCustomization", (h, v) =>
        {
            // Remove underline:
            h.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToAndroid());
            h.PlatformView.SetBackgroundColor(Colors.WhiteSmoke.ToAndroid());
            h.PlatformView.SetPadding(25, 5, 25, 5);
        });
#endif
    }
}
