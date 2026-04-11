using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Gergo3.OpenVPNCertificateManager;

public class DbUserService(IDbContextFactory<OpenVpnCertificateManagerContext> factory) : IUserService
{
    public async Task<ObservableCollection<User>> GetUsersAsync(Server server)
    {
        await using OpenVpnCertificateManagerContext context = await factory.CreateDbContextAsync();

        return new(await context.Users.Where(x => x.Server == server).ToArrayAsync());
    }

    public async Task AddUserAsync(User user)
    {
        await using OpenVpnCertificateManagerContext context = await factory.CreateDbContextAsync();
        
        context.Users.Add(user);
        
        await context.SaveChangesAsync();
    }
}