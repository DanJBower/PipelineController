using CommonWpf.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ControllerPassthroughClient;

public partial class MainViewModel : ViewModel
{

    [ObservableProperty]
    private ControllerViewModel _controllerViewModel = new();
}
