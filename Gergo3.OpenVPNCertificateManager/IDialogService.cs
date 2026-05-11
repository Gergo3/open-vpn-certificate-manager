using System.Threading.Tasks;

namespace Gergo3.OpenVPNCertificateManager;

public interface IDialogService
{
    /// <summary>
    /// Display a message in a dialog with a single ok button
    /// </summary>
    /// <param name="message">Message to display</param>
    /// <param name="title">Window title</param>
    /// <returns></returns>
    public Task ShowMessageAsync(string message, string title);
    
    /// <summary>
    /// Display an error in a dialog
    /// </summary>
    /// <param name="message">User readable short error message</param>
    /// <param name="errorMessage">Full error message</param>
    /// <returns></returns>
    public Task ShowErrorAsync(string message, string errorMessage);
    
    /// <summary>
    /// Display a confirmation dialog
    /// </summary>
    /// <param name="message">Message to display</param>
    /// <param name="title">Window title</param>
    /// <returns>true if ok, false if cancel</returns>
    public Task<bool> ShowConfirmationAsync(string message, string title);
    
}