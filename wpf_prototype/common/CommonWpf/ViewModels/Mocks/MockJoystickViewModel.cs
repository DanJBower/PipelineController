using CommonWpf.ViewModels.Interfaces;

namespace CommonWpf.ViewModels.Mocks;

public class MockJoystickViewModel : IJoystickViewModel
{
    public MockJoystickViewModel() { }

    public MockJoystickViewModel(string title,
        float xPosition,
        float yPosition,
        bool pressed)
    {
        Title = title;
        XPosition = xPosition;
        YPosition = yPosition;
        Pressed = pressed;
    }

    public string Title { get; } = "Left Stick";
    public float XPosition { get; } = 0.0f;
    public float YPosition { get; } = 0.0f;
    public bool Pressed { get; } = false;
}
