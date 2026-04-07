using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Gergo3.OpenVPNCertificateManager;

public partial class ServerEditWindow : Window
{
    public ServerEditWindow()
    {
        InitializeComponent();

        DataContext = new ServerEditWindowViewModel();
    }
}