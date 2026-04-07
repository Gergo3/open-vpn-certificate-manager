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
    private readonly MainWindow _mainWindow;

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

    public void EditServer(Server server)
    {
        
    }

    public async Task RemoveServerAsync()
    {
        Server server = SelectedServer ?? throw new ArgumentNullException(nameof(SelectedServer));
        await using var db = new OpenVpnCertificateManagerContext();
        db.Servers.Remove(server);
        await db.SaveChangesAsync();
        
        await RefreshServers();
    }

    public async Task AddServerAsync()
    {
        var data = await new AddServerPopup().ShowDialog<AddServerPopup.AddServerPopupResult>(_mainWindow);
        
        Server server = new (data.Name, data.Domain, data.Password);

        await using OpenVpnCertificateManagerContext db = new();
        
        db.Servers.Add(server);
        await db.SaveChangesAsync();
        
        
        await RefreshServers();
    }

    public async Task RefreshServers()
    {
        using OpenVpnCertificateManagerContext db = new();
        
        Servers = new(await db.Servers.ToArrayAsync());
    }

    public ViewModel(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        
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