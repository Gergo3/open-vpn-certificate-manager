using System;
using System.Threading.Tasks;

namespace Gergo3.OpenVPNCertificateManager;

public interface IServerExporterService
{
    /// <summary>
    /// Export a server
    /// </summary>
    /// <param name="server">The server to export</param>
    /// <param name="owner">owner window of the file picker dialog</param>
    /// <exception cref="PasswordNotSetException">Thrown if <see cref="Server.Password"/> is not set</exception>
    /// <exception cref="ArgumentNullException">Thrown if any argument is null</exception>
    public Task ExportServerAsync(Server server, object owner);
}