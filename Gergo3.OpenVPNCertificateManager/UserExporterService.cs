using System;
using System.IO;
using System.Threading.Tasks;

namespace Gergo3.OpenVPNCertificateManager;

public class UserExporterService(IDialogService dialogService) : IUserExporterService
{
    public async Task ExportUserAsync(User user, Server server, object owner)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(server);
        ArgumentNullException.ThrowIfNull(owner);

        string fileName = Path.Join(AppDir.OutputDir.FullName, user.Username.Replace(' ', '-') + ".ovpn");
        //string? fileName = (await dialogService.ShowSaveFileDialogAsync("Save User", "ovpn", owner))?.Path.LocalPath;
        //if (fileName is null) return;

        string data = $"""
                       client
                       dev {(server.Interface == Interface.Tun ? "tun" : "tap")}
                       proto {(server.Protocol == Protocol.Tcp ? "tcp" : "udp")}
                       remote {server.Domain} {server.Port}
                       
                       resolv-retry infinite
                       nobind
                       persist-key
                       persist-tun
                       
                       remote-cert-tls server
                       cipher AES-256-GCM
                       auth SHA256
                       
                       verb 3
                       
                       <ca>
                       {server.CaCrt}
                       </ca>
                       
                       <cert>
                       {user.Crt}
                       </cert>
                       
                       <key>
                       {user.Key}
                       </key>
                       """;
        
        await File.WriteAllTextAsync(fileName, data);
        _ = dialogService.ShowMessageAsync($"User exported as {fileName}", "User exported", owner);
    }
}