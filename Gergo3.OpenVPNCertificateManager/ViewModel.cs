using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Gergo3.OpenVPNCertificateManager;

public class ViewModel : INotifyPropertyChanged
{
    private readonly IWindowService _windowService;
    private readonly IServerService _serverService;

    public ObservableCollection<Server> Servers
    {
        get;
        set
        {
            if (Equals(value, field)) return;
            field = value;
            OnPropertyChanged();
        }
    } = new();

    public Server? SelectedServer
    {
        get;
        set
        {
            if (Equals(value, field)) return;
            field = value;
            OnPropertyChanged();
        }
    }

    public void EditServer()
    {
        if (SelectedServer == null) return;
        
        _windowService.ShowServerEditWindow(SelectedServer);
    }

    public async Task RemoveServerAsync()
    {
        Server server = SelectedServer ?? throw new ArgumentNullException(nameof(SelectedServer));
        
        await _serverService.RemoveServerAsync(server);
        
        await RefreshServers();
    }

    public async Task AddServerAsync()
    {
        var data = await _windowService.ShowAddServerPopupWindow<AddServerPopupResult>(this);
        //var data = await new AddServerPopup().ShowDialog<AddServerPopup.AddServerPopupResult>(_mainWindow);
        
        Server server = new (data.Name, data.Domain, data.Password);

        await _serverService.AddServerAsync(server);
        
        await RefreshServers();
    }

    public async Task RefreshServers()
    {
        Servers = await _serverService.GetServersAsync();
    }

    public ViewModel(IWindowService windowService, IServerService serverService)
    {
        _windowService  = windowService;
        _serverService = serverService;
        
        _ = RefreshServers();
        
    }
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