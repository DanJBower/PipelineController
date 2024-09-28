using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;

namespace CommonWpf.ViewModels;

public partial class FourButtonPanelViewModel : ViewModel
{
    [ObservableProperty]
    private string _groupTitle;

    [ObservableProperty]
    private string _buttonOneTitle;

    [ObservableProperty]
    private bool _buttonOnePressed;

    [ObservableProperty]
    private SolidColorBrush _buttonOneActiveColour;

    [ObservableProperty]
    private string _buttonTwoTitle;

    [ObservableProperty]
    private bool _buttonTwoPressed;

    [ObservableProperty]
    private SolidColorBrush _buttonTwoActiveColour;

    [ObservableProperty]
    private string _buttonThreeTitle;

    [ObservableProperty]
    private bool _buttonThreePressed;

    [ObservableProperty]
    private SolidColorBrush _buttonThreeActiveColour;

    [ObservableProperty]
    private string _buttonFourTitle;

    [ObservableProperty]
    private bool _buttonFourPressed;

    [ObservableProperty]
    private SolidColorBrush _buttonFourActiveColour;
}
