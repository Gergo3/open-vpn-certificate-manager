using System.Threading.Tasks;

namespace Gergo3.OpenVPNCertificateManager;

public interface IWindowService
{
    public void ShowServerEditWindow(Server server);
    
    public Task<TResult> ShowAddServerPopupWindow<TResult>(object viewModel);

    public Task<TResult> ShowDialog<TWindow, TResult>(object viewModel) where TWindow : IDialog;
}