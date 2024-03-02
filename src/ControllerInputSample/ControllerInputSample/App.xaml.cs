#if WINDOWS
using Windows.Graphics;
#endif
using Microsoft.Maui.Platform;

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


        window.Created += (_, _) =>
        {
            if (Preferences.Default.ContainsKey(LastWidthPropertyKey) &&
                Preferences.Default.ContainsKey(LastHeightPropertyKey))
            {
                window.Width = Preferences.Default.Get(LastWidthPropertyKey, -1.0);
                window.Height = Preferences.Default.Get(LastHeightPropertyKey, -1.0);
            }

            if (Preferences.Default.ContainsKey(LastXPropertyKey) &&
                Preferences.Default.ContainsKey(LastYPropertyKey))
            {
                var lastX = Preferences.Default.Get(LastXPropertyKey, 0.0);
                var lastY = Preferences.Default.Get(LastYPropertyKey, 0.0);
                var dpi = DeviceDisplay.MainDisplayInfo.Density;
                if (Math.Abs(DeviceDisplay.MainDisplayInfo.Density - 1) > 0.00001 &&
                    (lastX < 0 || lastX > (DeviceDisplay.MainDisplayInfo.Width / dpi) ||
                     lastY < 0 || lastY > (DeviceDisplay.MainDisplayInfo.Height / dpi)))
                {
                    var nativeWindow = (MauiWinUIWindow)window.Handler.PlatformView;
                    var appWindow = nativeWindow.GetAppWindow();
                    appWindow.Move(new PointInt32(
                        (int)Math.Round(lastX),
                        (int)Math.Round(lastY)
                    ));
                }
                else
                {
                    window.X = lastX;
                    window.Y = lastY;
                }
            }
        };

        window.Destroying += (_, _) =>
        {
            Preferences.Default.Set(LastWidthPropertyKey, window.Width);
            Preferences.Default.Set(LastHeightPropertyKey, window.Height);
            Preferences.Default.Set(LastXPropertyKey, window.X);
            Preferences.Default.Set(LastYPropertyKey, window.Y);
        };

        return window;
    }
#endif
}