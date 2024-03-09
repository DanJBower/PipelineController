#if WINDOWS
using Microsoft.Maui.Platform;
using Microsoft.UI.Windowing;
using Windows.Graphics;

namespace ControllerInputSampleMauiOnly;

public static class RememberWindowPosition
{
    private const string LastWidthPropertyKey = "windows_last_window_width";
    private const string LastHeightPropertyKey = "windows_last_window_height";
    private const string LastXPropertyKey = "windows_last_window_x";
    private const string LastYPropertyKey = "windows_last_window_y";
    private const string LastWindowMaximisedPropertyKey = "windows_last_window_state";

    public static Window MemoizeWindow(this Window window)
    {
        AppWindow appWindow = null!;
        OverlappedPresenter presenter = null!;

        window.Created += (_, _) =>
        {
            var nativeWindow = (MauiWinUIWindow)window.Handler!.PlatformView!;
            appWindow = nativeWindow.GetAppWindow()!;
            presenter = (OverlappedPresenter)appWindow.Presenter;

            if (Preferences.Default.ContainsKey(LastXPropertyKey) &&
                Preferences.Default.ContainsKey(LastYPropertyKey))
            {
                appWindow.Move(new PointInt32(
                    Preferences.Default.Get(LastXPropertyKey, 0),
                    Preferences.Default.Get(LastYPropertyKey, 0)
                ));
            }

            if (Preferences.Default.ContainsKey(LastWidthPropertyKey) &&
                Preferences.Default.ContainsKey(LastHeightPropertyKey))
            {
                window.Width = Preferences.Default.Get(LastWidthPropertyKey, -1.0);
                window.Height = Preferences.Default.Get(LastHeightPropertyKey, -1.0);
            }

            if (Preferences.Default.Get(LastWindowMaximisedPropertyKey, false))
            {
                presenter.Maximize();
            }
        };

        window.Destroying += (_, _) =>
        {
            Preferences.Default.Set(LastXPropertyKey, appWindow.Position.X);
            Preferences.Default.Set(LastYPropertyKey, appWindow.Position.Y);
            var isMaximised = presenter.State == OverlappedPresenterState.Maximized;
            Preferences.Default.Set(LastWindowMaximisedPropertyKey, isMaximised);

            if (!isMaximised)
            {
                Preferences.Default.Set(LastWidthPropertyKey, window.Width);
                Preferences.Default.Set(LastHeightPropertyKey, window.Height);
            }
        };

        return window;
    }
}
#endif
