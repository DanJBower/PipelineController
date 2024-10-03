using CommonWpf.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CommonWpf.ViewModels;

public partial class FourButtonPanelViewModel : ViewModel, IFourButtonPanelViewModel
{
    [ObservableProperty]
    private string _groupTitle;

    [ObservableProperty]
    private string _buttonOneTitle;

    [ObservableProperty]
    private bool _buttonOnePressed;

    [ObservableProperty]
    private string _buttonTwoTitle;

    [ObservableProperty]
    private bool _buttonTwoPressed;

    [ObservableProperty]
    private string _buttonThreeTitle;

    [ObservableProperty]
    private bool _buttonThreePressed;

    [ObservableProperty]
    private string _buttonFourTitle;

    [ObservableProperty]
    private bool _buttonFourPressed;
}
