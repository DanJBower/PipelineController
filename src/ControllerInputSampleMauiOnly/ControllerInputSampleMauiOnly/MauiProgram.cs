using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using ControllerInputSampleMauiOnly.ViewModels;
using Microsoft.Extensions.Logging;

namespace ControllerInputSampleMauiOnly;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMarkup()
            .RegisterDependencies()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static MauiAppBuilder RegisterDependencies(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.Scan(scan => scan.FromCallingAssembly()
            .AddClasses(classes => classes.AssignableTo<ViewModel>())
            .AsSelf()
            .WithTransientLifetime());
		// Look into if I should use the C# markup from MauiCommunityToolkitMarkup or use XAML
		// Error here: From CommunityToolkit having a add transient <TView, TViewModel> clashing with the normal add transient. May not use this, but need to investigate
		// Is sample of how they do it in Repos\NotMine\Maui\samples\CommunityToolkit.Maui.Sample
        mauiAppBuilder.Services.AddTransient<Page, AppShell>();
        return mauiAppBuilder;
    }
}