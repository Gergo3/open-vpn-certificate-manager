using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Gergo3.OpenVPNCertificateManager;

public partial class ServerEditWindow : Window, IServerEditWindow
{
    public ServerEditWindow(ServerEditWindowViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}