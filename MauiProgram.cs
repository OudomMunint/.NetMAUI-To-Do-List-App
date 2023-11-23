using CommunityToolkit.Maui;
using Maui.FreakyControls.Extensions;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Hosting;

namespace ToDoListApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder

            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Inter-Regular.ttf", "InterRegular");
                fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
                fonts.AddFont("Inter-Bold.ttf", "InterBold");
            });
            //.ConfigureMauiHandlers(handlers =>
            //    {
            //        handlers.AddHandler<Label>(handler =>
            //        {
            //            handler.UseDefaults = false;
            //            handler.FontAutoScalingEnabled = false;
            //        });
            //    });
        builder.InitializeFreakyControls();
        AndroidHandlers.Apply();

        return builder.Build();
    }
}