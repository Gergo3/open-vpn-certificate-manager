using Microsoft.EntityFrameworkCore;

namespace Gergo3.OpenVPNCertificateManager;

public class OpenVpnCertificateManagerContext : DbContext
{
    public DbSet<Server> Servers { get; set; }
    public DbSet<User> Users { get; set; }

    static OpenVpnCertificateManagerContext()
    {
        new OpenVpnCertificateManagerContext().Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite($"Data Source={AppDir.DbPath}");
    
}