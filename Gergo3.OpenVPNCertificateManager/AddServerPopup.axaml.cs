using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace Gergo3.OpenVPNCertificateManager;

public partial class AddServerPopup : Window, IDialog
{
    public string? NameInput { get; set; }
    public string? Domain { get; set; }
    public int? Port { get; set; }
    public Protocol? Protocol { get; set; }
    public Protocol[] Protocols => Enum.GetValues<Protocol>();
    public Interface? Interface { get; set; }
    public Interface[] Interfaces => Enum.GetValues<Interface>();
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
                Port =  Port ?? throw new InputNullException(),
                Protocol = Protocol ?? throw new InputNullException(),
                Interface = Interface ?? throw new InputNullException(),
            });
        }
        catch (InputNullException e)
        {
             MessageBoxManager.GetMessageBoxStandard("Error", e.ToString()).ShowWindowDialogAsync(this);
        }
    }


    public AddServerPopup()
    {
        InitializeComponent();

        DataContext = this;
    }
    
    private class InputNullException : Exception;
}