using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace CO.UI.Blazor.Shared.Utilities;

public class GlobalErrorBoundary : ErrorBoundary
{
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Inject] private ILogger<GlobalErrorBoundary> Logger { get; set; } = default!;

    protected override Task OnErrorAsync(Exception exception)
    {
        // 1. Log the actual technical error for developers
        Logger.LogError(exception, "Unhandled exception in Blazor circuit.");

        // 2. Show a user-friendly message via MudBlazor Snackbar
        Snackbar.Add("A technical error occurred. We've been notified.", Severity.Error, config =>
        {
            config.VisibleStateDuration = 10000; // Keep it visible longer
            config.HideIcon = false;
        });

        // 3. Optional: In development, you might want to call base to see the red box
        // In production, we return Task.CompletedTask to "swallow" the crash UI 
        // if we want to keep the user on the page.
        return Task.CompletedTask;
    }
}