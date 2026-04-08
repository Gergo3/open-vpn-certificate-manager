using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Gergo3.OpenVPNCertificateManager;

public partial class AddServerPopup : Window
{
    public string? NameInput { get; set; }
    public string? Domain { get; set; }
    public string? Password { get; set; }
    public string? PasswordConfirmation { get; set; }
    public void Ok()
    {
        if (Password != PasswordConfirmation) return;
        try
        {
            Close(new AddServerPopupResult
            {
                Name = NameInput ?? throw new InputNullException(),
                Domain = Domain ?? throw new InputNullException(),
                Password = Password ?? throw new InputNullException(),
            });
        }
        catch (InputNullException e)
        {
        }
    }


    public AddServerPopup()
    {
        InitializeComponent();

        DataContext = this;
    }
    
    private class InputNullException : Exception;
}