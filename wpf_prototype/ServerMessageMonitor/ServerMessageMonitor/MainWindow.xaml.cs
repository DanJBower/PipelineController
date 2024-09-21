using CommonClient;

namespace ServerMessageMonitor;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        ClientBasics.Test();
    }
}
