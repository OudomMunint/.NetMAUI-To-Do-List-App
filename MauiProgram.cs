using CommunityToolkit.Maui;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Hosting;
using The49.Maui.BottomSheet;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using ToDoListApp;
using System;
#if ANDROID
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
#endif
using Microcharts.Maui;

namespace ToDoListApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder

            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseBottomSheet()
            .UseMicrocharts()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Inter-Regular.ttf", "InterRegular");
                fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
                fonts.AddFont("Inter-Bold.ttf", "InterBold");
            })
            .ConfigureMauiHandlers(handlers =>
            {
#if ANDROID
                handlers.AddHandler<CustomViewCell, CustomViewCellHandler>();
#endif
            });
        //.ConfigureMauiHandlers(handlers =>
        //    {
        //        handlers.AddHandler<Label>(handler =>
        //        {
        //            handler.UseDefaults = false;
        //            handler.FontAutoScalingEnabled = false;
        //        });
        //    });
        AndroidHandlers.Apply();

        return builder.Build();
    }
}