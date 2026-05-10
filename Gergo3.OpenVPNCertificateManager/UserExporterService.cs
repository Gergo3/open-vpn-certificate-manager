using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls.Shapes;

namespace Gergo3.OpenVPNCertificateManager;

public class UserExporterService : IUserExporterService
{
    public async Task ExportUserAsync(User user)
    {
        string fileName = System.IO.Path.Join(AppDir.OutputDir.FullName, user.Username.Replace(' ', '-'));

        string data = $"""
                       client
                       dev {(user.Server.Interface == Interface.Tun ? "tun" : "tap")}
                       proto {(user.Server.Protocol == Protocol.Tcp ? "tcp" : "udp")}
                       remote {user.Server.Domain} {user.Server.Port}
                       
                       resolv-retry infinite
                       nobind
                       persist-key
                       persist-tun
                       
                       remote-cert-tls server
                       cipher AES-256-GCM
                       auth SHA256
                       
                       verb 3
                       
                       <ca>
                       -----BEGIN CERTIFICATE-----
                       {user.Server.CaCrt}
                       -----END CERTIFICATE-----
                       </ca>
                       
                       <cert>
                       -----BEGIN CERTIFICATE-----
                       {user.Crt}
                       -----END CERTIFICATE-----
                       </cert>
                       
                       <key>
                       -----BEGIN PRIVATE KEY-----
                       {user.Key}
                       -----END PRIVATE KEY-----
                       </key>
                       """;
        
        await File.WriteAllTextAsync(fileName, data);
    }
}