#if IOS
using Microsoft.Maui.Platform;
using UIKit;
using System.Drawing;

namespace ToDoListApp.Handlers
{
    public class IosKeyboardHandler : Microsoft.Maui.Handlers.EntryHandler
    {
        protected override void ConnectHandler(MauiTextField platformView)
        {
            base.ConnectHandler(platformView);

            var topBar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));
            var doneBtn = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
            {
                this.PlatformView.EndEditing(true);
            });

            topBar.Items = new UIBarButtonItem[]
            {
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), doneBtn
            };
            this.PlatformView.InputAccessoryView = topBar;
        }
    }
}
#endif