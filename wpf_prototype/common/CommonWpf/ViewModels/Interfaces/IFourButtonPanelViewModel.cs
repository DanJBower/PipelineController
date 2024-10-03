namespace CommonWpf.ViewModels.Interfaces;

public interface IFourButtonPanelViewModel
{
    public string GroupTitle { get; }

    public string ButtonOneTitle { get; }

    public bool ButtonOnePressed { get; }

    public string ButtonTwoTitle { get; }

    public bool ButtonTwoPressed { get; }

    public string ButtonThreeTitle { get; }

    public bool ButtonThreePressed { get; }

    public string ButtonFourTitle { get; }

    public bool ButtonFourPressed { get; }
}
