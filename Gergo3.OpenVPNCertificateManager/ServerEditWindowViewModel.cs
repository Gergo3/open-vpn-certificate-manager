using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Gergo3.OpenVPNCertificateManager;

public class ServerEditWindowViewModel(IUserService userService, IWindowService windowService, IServerExporterService serverExporterService, IUserExporterService userExporterService) : INotifyPropertyChanged
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

    public User? SelectedUser { get; set; }
    public async void AddUserAsync()
    {
        string name = await windowService.ShowDialog<AddUserPopup,string>(this);
        
        User user = Server.CreateUser(name);

        await userService.AddUserAsync(user);
    }
    
    public async void ExportServerAsync() => 
        await serverExporterService.ExportServerAsync(Server);

    public async void ExportUserAsync() => 
        await userExporterService.ExportUserAsync(SelectedUser);

    public async void RefreshUsersAsync() => 
        Users = await userService.GetUsersAsync(Server);

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
}