using CommonWpf.ViewModels.Interfaces;

namespace CommonWpf.ViewModels.Mocks;

public class MockControllerViewModel : IControllerViewModel
{
    public bool StartPressed => true;
    public string DPadUpTitle => "Up";
    public string DPadRightTitle => "Right";
    public string DPadDownTitle => "Down";
    public string DPadLeftTitle => "Left";
}
