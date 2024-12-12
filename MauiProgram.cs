﻿using CommunityToolkit.Maui;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using ToDoListApp;
using System;
using Microcharts.Maui;
#if ANDROID
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
#endif
#if DEBUG
using Maui.FreakyEffects;
using The49.Maui.BottomSheet;
using DotNet.Meteor.HotReload.Plugin;
using Plugin.Maui.Biometric;
#endif

namespace ToDoListApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // DI for biometricss
        builder.Services.AddSingleton<IBiometric>(BiometricAuthenticationService.Default);

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMicrocharts()
#if DEBUG
            .UseBottomSheet()
            .EnableHotReload()
            .ConfigureEffects(effects =>
            {
                effects.InitFreakyEffects();
            })
#endif
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Inter-Regular.ttf", "InterRegular");
                fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
                fonts.AddFont("Inter-Bold.ttf", "InterBold");
            })
            .ConfigureMauiHandlers(handlers =>
            {
#if ANDROID
                handlers.AddHandler<CustomViewCell, CustomViewCellHandler>();
#endif
            })
            .ConfigureEssentials(essentials =>
            {
                essentials.UseVersionTracking();
            });

        AndroidHandlers.Apply();

        return builder.Build();
    }
}