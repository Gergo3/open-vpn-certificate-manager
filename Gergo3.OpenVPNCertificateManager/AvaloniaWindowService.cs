using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;

namespace Gergo3.OpenVPNCertificateManager;

public class AvaloniaWindowService(IServiceProvider provider) : IWindowService
{
    private Window GetParentWindow(object viewModel) =>
        Application.Current?.ApplicationLifetime
            is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.Windows.Single(x => x.DataContext == viewModel)
            : throw new InvalidOperationException("No desktop lifetime");

    public void ShowServerEditWindow(Server server)
    {
        ArgumentNullException.ThrowIfNull(server);
        var window = provider.GetRequiredService<ServerEditWindow>();
        if (window.DataContext is ServerEditWindowViewModel viewModel)
            viewModel.Server = server;
        else throw new InvalidOperationException("Invalid view model");
        window.Show();
    }

    public Task<TResult> ShowAddServerPopupWindow<TResult>(object viewModel) => 
        provider
            .GetRequiredService<AddServerPopup>()
            .ShowDialog<TResult>(GetParentWindow(viewModel));

    public Task<TResult> ShowDialog<TWindow, TResult>(object viewModel) where TWindow : IDialog =>
        provider.GetRequiredService<TWindow>() is Window window
            ? window.ShowDialog<TResult>(GetParentWindow(viewModel))
            : throw new ArgumentException("Window must be avalonia window", nameof(TWindow));
}