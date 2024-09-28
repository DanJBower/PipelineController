using CommonWpf.ViewModels.Interfaces;
using System.Windows.Media;

namespace CommonWpf.ViewModels.Mocks;

public class MockFourButtonPanelViewModel : IFourButtonPanelViewModel
{
    public MockFourButtonPanelViewModel() { }

    public MockFourButtonPanelViewModel(string groupTitle,
        string buttonOneTitle,
        bool buttonOnePressed,
        SolidColorBrush buttonOneActiveColour,
        string buttonTwoTitle,
        bool buttonTwoPressed,
        SolidColorBrush buttonTwoActiveColour,
        string buttonThreeTitle,
        bool buttonThreePressed,
        SolidColorBrush buttonThreeActiveColour,
        string buttonFourTitle,
        bool buttonFourPressed,
        SolidColorBrush buttonFourActiveColour)
    {
        GroupTitle = groupTitle;

        ButtonOneTitle = buttonOneTitle;
        ButtonOnePressed = buttonOnePressed;
        ButtonOneActiveColour = buttonOneActiveColour;

        ButtonTwoTitle = buttonTwoTitle;
        ButtonTwoPressed = buttonTwoPressed;
        ButtonTwoActiveColour = buttonTwoActiveColour;

        ButtonThreeTitle = buttonThreeTitle;
        ButtonThreePressed = buttonThreePressed;
        ButtonThreeActiveColour = buttonThreeActiveColour;

        ButtonFourTitle = buttonFourTitle;
        ButtonFourPressed = buttonFourPressed;
        ButtonFourActiveColour = buttonFourActiveColour;
    }

    public string GroupTitle { get; } = "Button Group Title";

    public string ButtonOneTitle { get; } = "Medium";
    public bool ButtonOnePressed { get; } = false;
    public SolidColorBrush ButtonOneActiveColour { get; } = Brushes.DarkRed;

    public string ButtonTwoTitle { get; } = "1";
    public bool ButtonTwoPressed { get; } = true;
    public SolidColorBrush ButtonTwoActiveColour { get; } = Brushes.DarkGreen;

    public string ButtonThreeTitle { get; } = "Pretty Long";
    public bool ButtonThreePressed { get; } = false;
    public SolidColorBrush ButtonThreeActiveColour { get; } = Brushes.DarkBlue;

    public string ButtonFourTitle { get; } = "2";
    public bool ButtonFourPressed { get; } = true;
    public SolidColorBrush ButtonFourActiveColour { get; } = Brushes.DarkMagenta;
}
