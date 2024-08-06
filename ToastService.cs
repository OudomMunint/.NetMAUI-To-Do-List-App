using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace ToDoListApp
{
    public static class ToastService
    {
        public static async Task ShowToastAsync(string message, ToastDuration duration = ToastDuration.Short, int fontSize = 16)
        {
            CancellationTokenSource cancellationTokenSource = new();
            var toast = Toast.Make(message, duration, fontSize);
            await toast.Show(cancellationTokenSource.Token);
        }
    }
}