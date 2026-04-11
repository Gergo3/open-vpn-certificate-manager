using System;
using Microsoft.Extensions.DependencyInjection;

namespace Gergo3.OpenVPNCertificateManager;

public static class AppHost
{
    public static IServiceProvider Build(Action<IServiceCollection> configure)
    {
        ServiceCollection services = new();
        
        //db
        services.AddDbContextFactory<OpenVpnCertificateManagerContext>();
        
        //services
        services.AddTransient<IServerService, DbServerService>();
        services.AddTransient<IUserService,DbUserService>();
        services.AddTransient<IServerExporterService, ServerExporterService>();
        services.AddTransient<IUserExporterService, UserExporterService>();
        
        //ViewModels
        services.AddTransient<ViewModel>();
        services.AddTransient<ServerEditWindowViewModel>();
        
        //Gui framework
        configure(services);
        
        return services.BuildServiceProvider();
    }
}