using CO.UI.Blazor.Contracts.Interfaces.Services;
using MudBlazor;

namespace CO.UI.Blazor.Contracts.Implementations.Services;

internal sealed class AppNotificationService(ISnackbar snackbar) : IAppNotificationService
{
    public void ShowError(string message) =>
        snackbar.Add(message, Severity.Error);

    public void ShowSuccess(string message) =>
        snackbar.Add(message, Severity.Success);

    public void ShowWarning(string message) =>
        snackbar.Add(message, Severity.Warning);
}
