namespace CommonWpf.ViewModels.Interfaces;

public interface IControllerViewModel
{
    bool StartPressed { get; }
    string DPadUpTitle { get; }
    string DPadRightTitle { get; }
    string DPadDownTitle { get; }
    string DPadLeftTitle { get; }
}
