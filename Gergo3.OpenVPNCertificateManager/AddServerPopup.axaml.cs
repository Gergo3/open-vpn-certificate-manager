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
        try
        {
            if (Password != PasswordConfirmation) throw new PasswordDoesntMatchException();
            if (string.IsNullOrWhiteSpace(NameInput) ||
                string.IsNullOrWhiteSpace(Domain) ||
                string.IsNullOrWhiteSpace(Password))
                throw new InputNullException();

            Close(new AddServerPopupResult
            {
                Name = NameInput ?? throw new InputNullException(),
                Domain = Domain ?? throw new InputNullException(),
                Password = Password ?? throw new InputNullException(),
                Port = Port ?? throw new InputNullException(),
                Protocol = Protocol ?? throw new InputNullException(),
                Interface = Interface ?? throw new InputNullException(),
            });
        }
        catch (InputNullException e)
        {
            _dialogService.ShowMessageAsync("Fill out all fields", "Fill out all fields", this);
        }
        catch (PasswordDoesntMatchException e)
        {
            _dialogService.ShowMessageAsync("Password doesnt match", "Password doesnt match", this);
        }
    }


    public AddServerPopup(IDialogService dialogService)
    {
        InitializeComponent();

        DataContext = this;
        
        _dialogService= dialogService;
    }
    
    private readonly IDialogService _dialogService;
    
    private class InputNullException : Exception;

    private class PasswordDoesntMatchException : Exception;
}