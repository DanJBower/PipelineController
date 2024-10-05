namespace CommonWpf.ViewModels.Interfaces;

public interface IFourButtonPanelViewModel
{
    string GroupTitle { get; }

    string ButtonOneTitle { get; }

    bool ButtonOnePressed { get; }

    string ButtonTwoTitle { get; }

    bool ButtonTwoPressed { get; }

    string ButtonThreeTitle { get; }

    bool ButtonThreePressed { get; }

    string ButtonFourTitle { get; }

    bool ButtonFourPressed { get; }
}
