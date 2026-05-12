using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Gergo3.OpenVPNCertificateManager;

public class DbServerService(IDbContextFactory<OpenVpnCertificateManagerContext> factory) : IServerService
{
    public async Task<ObservableCollection<Server>> GetServersAsync()
    {
        await using OpenVpnCertificateManagerContext db = await factory.CreateDbContextAsync();
        
        return new(await db.Servers.ToListAsync());
    }

    public async Task AddServerAsync(Server server)
    {
        await using OpenVpnCertificateManagerContext db = await factory.CreateDbContextAsync();
        
        db.Servers.Add(server);
        await db.SaveChangesAsync();
    }

    public async Task RemoveServerAsync(Server server)
    {
        await using OpenVpnCertificateManagerContext db = await factory.CreateDbContextAsync();
        db.Servers.Remove(server);
        try
        {
            int affected = await db.SaveChangesAsync();
            if (affected == 0)
            {
                throw new InvalidOperationException("Server does not exist, or was deleted");
            }
        }
        catch (DbUpdateConcurrencyException e)
        {
            throw new InvalidOperationException("Server does not exist, or was deleted");
        }
    }
    
}