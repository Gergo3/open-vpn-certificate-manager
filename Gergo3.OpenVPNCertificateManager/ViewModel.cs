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
    private readonly IDialogService _dialogService;



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
        try
        {
            Server server = SelectedServer ?? throw new ArgumentNullException(nameof(SelectedServer));
            
            if (!await _dialogService.ShowConfirmationAsync($"Are you sure you want to remove {server.Name}?", "Remove server?", this)) return;

            await _serverService.RemoveServerAsync(server);
        }
        catch (ArgumentNullException e)
        {
            Console.Error.WriteLine(e);
            _ = _dialogService
                .ShowMessageAsync("Select a server first", "No server selected", this);
        }
        catch (InvalidOperationException e)
        {
            Console.Error.WriteLine(e);
            _ = _dialogService.ShowErrorAsync
                ("The server does not exist, or was deleted", e.ToString(), this);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            _ = _dialogService.ShowErrorAsync(e.Message, e.ToString(), this);
        }
        
        await RefreshServers();
    }

    public async Task AddServerAsync()
    {
        var data = await _windowService.ShowAddServerPopupWindow<AddServerPopupResult>(this);
        //var data = await new AddServerPopup().ShowDialog<AddServerPopup.AddServerPopupResult>(_mainWindow);
        
        Server server = new (data.Name, data.Domain, data.Password, data.Interface, data.Protocol, data.Port);

        await _serverService.AddServerAsync(server);
        
        await RefreshServers();
    }

    public async Task RefreshServers()
    {
        try
        {
            Servers = await _serverService.GetServersAsync();
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            //RefreshServers lefut mielőtt dialoge ablakot lehet megjeleníteni
            //_ = _dialogService.ShowErrorAsync(e.Message, e.ToString(), this);
        }
    }

    public ViewModel(IWindowService windowService, IServerService serverService, IDialogService dialogService)
    {
        _windowService  = windowService;
        _serverService = serverService;
        _dialogService = dialogService;
        
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