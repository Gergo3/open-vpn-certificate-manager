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

    public void ShowServerEditWindow()
    {
        throw new System.NotImplementedException();
    }

    public Task<TResult> ShowAddServerPopupWindow<TResult>(object viewModel) => 
        provider
            .GetRequiredService<AddServerPopup>()
            .ShowDialog<TResult>(GetParentWindow(viewModel));
}