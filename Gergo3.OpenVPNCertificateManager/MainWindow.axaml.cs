using Avalonia.Controls;

namespace Gergo3.OpenVPNCertificateManager;

public partial class MainWindow : Window , IMainWindow
{
    public MainWindow(ViewModel viewModel)
    {
        InitializeComponent();
        
        DataContext = viewModel;
    }
}