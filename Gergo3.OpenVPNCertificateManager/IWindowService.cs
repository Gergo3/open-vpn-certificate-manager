using System.Threading.Tasks;

namespace Gergo3.OpenVPNCertificateManager;

public interface IWindowService
{
    public void ShowServerEditWindow();
    
    public Task<TResult> ShowAddServerPopupWindow<TResult>(object viewModel);
}