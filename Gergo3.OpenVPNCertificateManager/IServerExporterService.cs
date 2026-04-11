using System.Threading.Tasks;

namespace Gergo3.OpenVPNCertificateManager;

public interface IServerExporterService
{
    public Task ExportServerAsync(Server server);
}