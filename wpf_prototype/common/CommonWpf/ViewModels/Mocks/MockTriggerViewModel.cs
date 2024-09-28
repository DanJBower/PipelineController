using CommonWpf.ViewModels.Interfaces;

namespace CommonWpf.ViewModels.Mocks;

public class MockTriggerViewModel : ITriggerViewModel
{
    public MockTriggerViewModel() { }

    public MockTriggerViewModel(string title,
        float triggerValue)
    {
        Title = title;
        TriggerValue = triggerValue;
    }

    public string Title { get; } = "Left Trigger";
    public float TriggerValue { get; } = 0.56f;
}
