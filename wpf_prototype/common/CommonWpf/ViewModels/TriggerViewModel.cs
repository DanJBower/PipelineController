using CommonWpf.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CommonWpf.ViewModels;

public partial class TriggerViewModel : ViewModel, ITriggerViewModel
{
    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private float _triggerValue;
}
