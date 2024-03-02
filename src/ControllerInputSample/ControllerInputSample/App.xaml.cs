﻿#if WINDOWS
using Microsoft.Maui.Platform;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif

namespace ControllerInputSample;

public partial class App
{
    public App()
    {
        InitializeComponent();

        MainPage = new MainPage();
    }

#if WINDOWS
    private const string LastWidthPropertyKey = "windows_last_window_width";
    private const string LastHeightPropertyKey = "windows_last_window_height";
    private const string LastXPropertyKey = "windows_last_window_x";
    private const string LastYPropertyKey = "windows_last_window_y";

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);
        AppWindow appWindow = null!;

        window.Created += (_, _) =>
        {
            var nativeWindow = (MauiWinUIWindow)window.Handler!.PlatformView!;
            appWindow = nativeWindow.GetAppWindow()!;

            if (Preferences.Default.ContainsKey(LastWidthPropertyKey) &&
                Preferences.Default.ContainsKey(LastHeightPropertyKey))
            {
                window.Width = Preferences.Default.Get(LastWidthPropertyKey, -1.0);
                window.Height = Preferences.Default.Get(LastHeightPropertyKey, -1.0);
            }

            if (Preferences.Default.ContainsKey(LastXPropertyKey) &&
                Preferences.Default.ContainsKey(LastYPropertyKey))
            {
                appWindow.Move(new PointInt32(
                    Preferences.Default.Get(LastXPropertyKey, 0),
                    Preferences.Default.Get(LastYPropertyKey, 0)
                ));
            }
        };

        window.Destroying += (_, _) =>
        {
            Preferences.Default.Set(LastWidthPropertyKey, window.Width);
            Preferences.Default.Set(LastHeightPropertyKey, window.Height);
            Preferences.Default.Set(LastXPropertyKey, appWindow.Position.X);
            Preferences.Default.Set(LastYPropertyKey, appWindow.Position.Y);
        };

        return window;
    }
#endif
}
