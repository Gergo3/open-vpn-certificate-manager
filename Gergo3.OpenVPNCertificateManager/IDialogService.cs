using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace Gergo3.OpenVPNCertificateManager;

public interface IDialogService
{
    /// <summary>
    /// Display a message in a dialog with a single ok button
    /// </summary>
    /// <param name="message">Message to display</param>
    /// <param name="title">Window title</param>
    /// <param name="owner">owner window viewmodel</param>
    public Task ShowMessageAsync(string message, string title, object owner);

    /// <summary>
    /// Display an error in a dialog
    /// </summary>
    /// <param name="message">User readable short error message</param>
    /// <param name="errorMessage">Full error message</param>
    /// <param name="owner">owner window viewmodel</param>
    public Task ShowErrorAsync(string message, string errorMessage, object owner);

    /// <summary>
    /// Display a confirmation dialog
    /// </summary>
    /// <param name="message">Message to display</param>
    /// <param name="title">Window title</param>
    /// <param name="owner">owner window viewmodel</param>
    /// <returns>true if ok, false if cancel</returns>
    public Task<bool> ShowConfirmationAsync(string message, string title, object owner);
    
    /// <summary>
    /// Open a file save dialog
    /// </summary>
    /// <param name="title">Window title</param>
    /// <param name="type">File type</param>
    /// <param name="owner">Owner window viewmodel</param>
    /// <returns>Selected file, or null if canceled</returns>
    /// <exception cref="ArgumentNullException">Thrown if <see cref="TopLevel"/> of <paramref name="owner"/> is null</exception>
    public Task<IStorageFile?> ShowSaveFileDialogAsync(string title, string type, object owner);
}