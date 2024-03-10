using ControllerInputSampleMauiOnly.ViewModels;

namespace ControllerInputSampleMauiOnly;

public static class Dependencies
{
    public static MauiAppBuilder RegisterDependencies(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.Scan(scan => scan.FromCallingAssembly()
            .AddClasses(classes => classes.AssignableTo<ViewModel>())
            .AsSelf()
            .WithTransientLifetime());

        mauiAppBuilder.Services.AddTransient<Page, AppShell>();
        return mauiAppBuilder;
    }
}
