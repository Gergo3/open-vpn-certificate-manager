using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Shapes;

namespace Gergo3.OpenVPNCertificateManager;

public class UserExporterService : IUserExporterService
{
    public async Task ExportUserAsync(User user, Server server)
    {
        ArgumentNullException.ThrowIfNull(user);

        string fileName = System.IO.Path.Join(AppDir.OutputDir.FullName, user.Username.Replace(' ', '-') + ".ovpn");

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
    }
}