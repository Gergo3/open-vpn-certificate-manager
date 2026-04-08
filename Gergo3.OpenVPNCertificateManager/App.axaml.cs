using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;

namespace Gergo3.OpenVPNCertificateManager;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var provider = AppHost.Build(x =>
        {
            //services
            x.AddTransient<IWindowService,AvaloniaWindowService>();
            
            //windows
            x.AddTransient<MainWindow>();
            x.AddTransient<IServerEditWindow,ServerEditWindow>();
            x.AddTransient<AddServerPopup>();
        });
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = provider.GetRequiredService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}