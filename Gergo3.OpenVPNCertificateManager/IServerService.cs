using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Gergo3.OpenVPNCertificateManager;

public interface IServerService
{
    /// <summary>
    /// Get all servers
    /// </summary>
    /// <returns>An ObservableCollection of all servers</returns>
    public Task<ObservableCollection<Server>> GetServersAsync();

    /// <summary>
    /// Add a new server
    /// </summary>
    /// <param name="server">The server to add</param>
    public Task AddServerAsync(Server server);

    /// <summary>
    /// Remove an existing server
    /// </summary>
    /// <param name="server">The existing server to remove</param>
    /// <exception cref="InvalidOperationException">Thrown when specified server does not exist</exception>
    public Task RemoveServerAsync(Server server);
}