namespace CommonWpf.ViewModels.Interfaces;

public interface IJoystickViewModel
{
    public string Title { get; }

    public float XPosition { get; }

    public float YPosition { get; }

    public bool PressedIn { get; }
}
