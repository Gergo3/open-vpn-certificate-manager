using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
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
            
            ButtonDefinitions = 
            [
                new()
                {
                    Name = "Ok",
                    IsDefault = true,
                    IsCancel = false,
                },
            ],
        }).ShowWindowDialogAsync(AvaloniaWindowService.GetParentWindow(parent));

    public async Task<bool> ShowConfirmationAsync(string message, string title,  object parent)
    {
        return (await MessageBoxManager.GetMessageBoxStandard(title, message, ButtonEnum.OkCancel)
            .ShowWindowDialogAsync(AvaloniaWindowService.GetParentWindow(parent))) == ButtonResult.Ok;
    }

    public Task<IStorageFile?> ShowSaveFileDialogAsync(string title, string type, object owner) =>
        (TopLevel.GetTopLevel(AvaloniaWindowService.GetParentWindow(owner)) 
         ?? throw new ArgumentNullException(nameof(owner), "toplevel for owner is null"))
        .StorageProvider.SaveFilePickerAsync(new()
        {
            Title =  title,
            DefaultExtension =  type,
            
            FileTypeChoices = 
            [ 
                new($"{type} Files")
                {
                    Patterns = [$"*.{type}"],
                },
            ],
            
            ShowOverwritePrompt = true,
        });
}