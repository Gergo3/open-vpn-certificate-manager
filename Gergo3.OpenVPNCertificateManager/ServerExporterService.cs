using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ZipFile = System.IO.Compression.ZipFile;

namespace Gergo3.OpenVPNCertificateManager;

public class ServerExporterService : IServerExporterService
{
    public async Task ExportServerAsync(Server server)
    {
        string caPem = server.CaCrt;

        string serverPem = server.ServerCrt;

        string serverKey = server.ServerKey;

        string filePath = Path.Join(AppDir.OutputDir.FullName, $"{server.Name}-server.zip");

        DirectoryInfo exportDir = AppDir.TempDir.CreateSubdirectory(server.Name);

        Task[] fileWries = 
        [
            File.WriteAllTextAsync(Path.Join(exportDir.FullName, "ca.crt"), caPem),
            File.WriteAllTextAsync(Path.Join(exportDir.FullName, "server.crt"), serverPem)
        ];
        
        await Task.WhenAll(fileWries);

        await Task.Run(async () =>
        {
            ZipFile.CreateFromDirectory(exportDir.FullName, filePath);
            
            using ZipArchive zip = ZipFile.Open(filePath, ZipArchiveMode.Update);
            ZipArchiveEntry entry = zip.CreateEntry("server.key");

            await using Stream stream = entry.Open();
            await stream.WriteAsync(Encoding.UTF8.GetBytes(serverKey));
        });
    }
}