using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace ToDoListApp
{
    public static class ToastService
    {
        public static async Task ShowToastAsync(string message, int fontSize, ToastDuration duration = ToastDuration.Short)
        {
            await Task.Delay(500);
            CancellationTokenSource cancellationTokenSource = new();
            var toast = Toast.Make(message, duration, fontSize);
            await toast.Show(cancellationTokenSource.Token);
        }
    }
}