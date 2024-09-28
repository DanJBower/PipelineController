using CommunityToolkit.Mvvm.ComponentModel;

namespace CommonWpf.ViewModels;

public partial class TriggerViewModel : ViewModel
{
    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private float _triggerValue;
}
