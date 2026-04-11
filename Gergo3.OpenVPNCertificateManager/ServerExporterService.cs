using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Gergo3.OpenVPNCertificateManager;

public class ServerExporterService : IServerExporterService
{
    public async Task ExportServerAsync(Server server)
    {
        string caPem = 
            "-----BEGIN CERTIFICATE-----\n" +
            Convert.ToBase64String(
                server.CaCert.Export(X509ContentType.Cert),
                Base64FormattingOptions.InsertLineBreaks) +
            "\n-----END CERTIFICATE-----";
        
        string serverPem = 
            "-----BEGIN CERTIFICATE-----\n" +
            Convert.ToBase64String(
                server.ServerCert.Export(X509ContentType.Cert),
                Base64FormattingOptions.InsertLineBreaks) +
            "\n-----END CERTIFICATE-----";
        
        string serverKey = 
            "-----BEGIN CERTIFICATE-----\n" +
            Convert.ToBase64String(
                server.ServerCert.GetRSAPrivateKey()?.ExportPkcs8PrivateKey() ?? throw new FormatException("Missing private key"),
                Base64FormattingOptions.InsertLineBreaks) +
            "\n-----END CERTIFICATE-----";

        string filePath = Path.Join(AppDir.OutputDir.FullName, $"{server.Name}-server.zip");

    }
}