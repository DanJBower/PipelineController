using CommonWpf.ViewModels.Interfaces;

namespace CommonWpf.ViewModels.Mocks;

public class MockFourButtonPanelViewModel : IFourButtonPanelViewModel
{
    public MockFourButtonPanelViewModel() { }

    public MockFourButtonPanelViewModel(string groupTitle,
        string buttonOneTitle,
        bool buttonOnePressed,
        string buttonTwoTitle,
        bool buttonTwoPressed,
        string buttonThreeTitle,
        bool buttonThreePressed,
        string buttonFourTitle,
        bool buttonFourPressed)
    {
        GroupTitle = groupTitle;

        ButtonOneTitle = buttonOneTitle;
        ButtonOnePressed = buttonOnePressed;

        ButtonTwoTitle = buttonTwoTitle;
        ButtonTwoPressed = buttonTwoPressed;

        ButtonThreeTitle = buttonThreeTitle;
        ButtonThreePressed = buttonThreePressed;

        ButtonFourTitle = buttonFourTitle;
        ButtonFourPressed = buttonFourPressed;
    }

    public string GroupTitle { get; } = "Button Group Title";

    public string ButtonOneTitle { get; } = "Medium";
    public bool ButtonOnePressed { get; } = false;

    public string ButtonTwoTitle { get; } = "1";
    public bool ButtonTwoPressed { get; } = true;

    public string ButtonThreeTitle { get; } = "Pretty Long";
    public bool ButtonThreePressed { get; } = false;

    public string ButtonFourTitle { get; } = "2";
    public bool ButtonFourPressed { get; } = true;
}
