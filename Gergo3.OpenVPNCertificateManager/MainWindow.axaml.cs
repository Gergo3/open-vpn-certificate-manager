using Avalonia.Controls;

namespace Gergo3.OpenVPNCertificateManager;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        DataContext = new ViewModel(this);
    }
}