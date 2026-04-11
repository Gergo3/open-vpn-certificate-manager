using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Gergo3.OpenVPNCertificateManager;

public partial class AddUserPopup : Window, IDialog
{
    public AddUserPopup()
    {
        InitializeComponent();
        
        DataContext = this;
    }
    
    public string? Username { get; set; }

    public void Ok() => 
        Close(Username);
}