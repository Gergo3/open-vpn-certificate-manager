using System.Threading.Tasks;
using Avalonia.Controls;
using DialogHostAvalonia;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace Gergo3.OpenVPNCertificateManager;

public class AvaloniaDialogService : IDialogService
{
    public Task ShowMessageAsync(string message, string title, object parent) =>
        MessageBoxManager.GetMessageBoxStandard(title, message)
            .ShowWindowDialogAsync(AvaloniaWindowService.GetParentWindow(parent));

    public Task ShowErrorAsync(string message, string errorMessage, object parent) =>
        MessageBoxManager.GetMessageBoxCustom(new()
        {
            ContentTitle = "Error",
            ContentHeader = "Error",
            ContentMessage = message,
            Markdown = false,
        }).ShowWindowDialogAsync(AvaloniaWindowService.GetParentWindow(parent));

    public async Task<bool> ShowConfirmationAsync(string message, string title,  object parent)
    {
        return (await MessageBoxManager.GetMessageBoxStandard(title, message, ButtonEnum.OkCancel)
            .ShowWindowDialogAsync(AvaloniaWindowService.GetParentWindow(parent))) == ButtonResult.Ok;
    }
}