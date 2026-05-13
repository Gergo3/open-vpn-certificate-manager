using System;
using System.Threading.Tasks;

namespace Gergo3.OpenVPNCertificateManager;

public interface IUserExporterService
{
    /// <summary>
    /// Export a user
    /// </summary>
    /// <param name="user">The user to export</param>
    /// <param name="server">the server the user belongs to</param>
    /// <param name="owner">owner window of the file picker dialog</param>
    /// <exception cref="PasswordNotSetException">Thrown if <see cref="Server.Password"/>, or <see cref="User.Password"/> is not set</exception>
    /// <exception cref="ArgumentNullException">Thrown if any argument is null</exception>
    public Task ExportUserAsync(User user, Server server, object owner);
    
}