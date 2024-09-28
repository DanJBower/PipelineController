using System.Windows.Media;

namespace CommonWpf.ViewModels.Interfaces;

public interface IFourButtonPanelViewModel
{
    public string GroupTitle { get; }

    public string ButtonOneTitle { get; }

    public bool ButtonOnePressed { get; }

    public SolidColorBrush ButtonOneActiveColour { get; }

    public string ButtonTwoTitle { get; }

    public bool ButtonTwoPressed { get; }

    public SolidColorBrush ButtonTwoActiveColour { get; }

    public string ButtonThreeTitle { get; }

    public bool ButtonThreePressed { get; }

    public SolidColorBrush ButtonThreeActiveColour { get; }

    public string ButtonFourTitle { get; }

    public bool ButtonFourPressed { get; }

    public SolidColorBrush ButtonFourActiveColour { get; }
}
