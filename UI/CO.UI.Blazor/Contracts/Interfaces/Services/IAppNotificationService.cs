namespace CO.UI.Blazor.Contracts.Interfaces.Services;

public interface IAppNotificationService
{
    void ShowError(string message);
    void ShowSuccess(string message);
    void ShowWarning(string message);
}
