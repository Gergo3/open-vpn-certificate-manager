using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Gergo3.OpenVPNCertificateManager;

public interface IUserService
{
    public Task<ObservableCollection<User>> GetUsersAsync(Server server);
    
    public Task AddUserAsync(User user);
    
}