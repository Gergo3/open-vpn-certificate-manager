using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Gergo3.OpenVPNCertificateManager;

public class ServerEditWindowViewModel(IUserService userService, IWindowService windowService, IServerExporterService serverExporterService, IUserExporterService userExporterService, IDialogService dialogService) : INotifyPropertyChanged
{
    public string? Password
    {
        get;
        set
        {
            field = value;
            Server.Password = value;
            OnPropertyChanged();
        }
    }
    
    
    public Server? Server { get; set; }
    
    
    public ObservableCollection<User> Users
    {
        get;
        set
        {
            if (Equals(value, field)) return;
            field = value;
            OnPropertyChanged();
        }
    } = [];
    

    public User? SelectedUser
    {
        get
        {
            User? user = field;
            user?.Password = Password;
            return user;
        }
        set;
    }
    
    
    

    public async Task AddUserAsync()
    {
        try
        {
            string? name = await windowService.ShowDialog<AddUserPopup, string?>(this);
            if (name is null) return;
            if (string.IsNullOrWhiteSpace(name))
            {
                _ = dialogService.ShowMessageAsync("Name can not be empty", "Name cannot be empty", this);
                return;
            }

            Server server = Server ?? throw new InvalidOperationException("Server is not set");

            User user = Server.CreateUser(name);

            await userService.AddUserAsync(user);
        }
        catch (PasswordNotSetException e)
        {
            _ = dialogService.ShowMessageAsync("Password not set", "Password not set", this);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            _ = dialogService.ShowErrorAsync(e.Message, e.ToString(), this);
        }
        
        await RefreshUsersAsync();
    }
    
    
    public async Task ExportServerAsync()
    {
        try
        {
            await serverExporterService.ExportServerAsync(Server, this);
        }
        catch (PasswordNotSetException e)
        {
            _ = dialogService.ShowMessageAsync("Password not set", "Password not set", this);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            _ = dialogService.ShowErrorAsync(e.Message, e.ToString(), this);
        }
    }


    public async Task ExportUserAsync()
    {
        try
        {
            if (SelectedUser is null) return;
            _ = Server ?? throw new InvalidOperationException("Server is not set");

            await userExporterService.ExportUserAsync(SelectedUser, Server, this);

        }
        catch (PasswordNotSetException e)
        {
            _ = dialogService.ShowMessageAsync("Password not set", "Password not set", this);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            _ = dialogService.ShowErrorAsync(e.Message, e.ToString(), this);
        }
    }


    public async Task RefreshUsersAsync() => 
        Users = await userService.GetUsersAsync(Server);

    
    
    
    
    #region PropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
    #endregion
}