namespace CommonWpf.ViewModels.Interfaces;

public interface IJoystickViewModel
{
    string Title { get; }

    float XPosition { get; }

    float YPosition { get; }

    bool Pressed { get; }
}
