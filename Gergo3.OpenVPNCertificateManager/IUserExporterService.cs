using System.Threading.Tasks;

namespace Gergo3.OpenVPNCertificateManager;

public interface IUserExporterService
{
    public Task ExportUserAsync(User user);
    
}