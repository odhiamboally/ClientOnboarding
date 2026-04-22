using CO.Application.Contracts.Interfaces.Services;
using CO.Application.Features.Dashboard.Queries;
using CO.Shared.Dtos.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CO.UI.Blazor.Features.Dashboard.Pages;

public class DashboardBase : ComponentBase
{
    [Inject] protected ISender Sender { get; set; } = default!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;
    [Inject] protected IUserService UserService { get; set; } = default!;

    protected DashboardSummaryResponse? _summary;
    protected bool _loading;

    protected override async Task OnInitializedAsync()
    {
        await LoadSummaryAsync();
    }

    private async Task LoadSummaryAsync()
    {
        _loading = true;

        try
        {
            var result = await Sender.Send(new GetDashboardSummaryQuery(UserService.UserId));

            if (result.Successful && result.Data is not null)
                _summary = result.Data;
            else
                Snackbar.Add(result.Message ?? "Failed to load dashboard.", Severity.Error);
        }
        catch (Exception)
        {
            Snackbar.Add("Dashboard unavailable.", Severity.Error);
        }
        finally
        {
            _loading = false;
            StateHasChanged();
        }
    }

    protected static int Pct(int part, int total)
        => total == 0 ? 0 : (int)Math.Round(part * 100.0 / total);

    private static int Max(IReadOnlyList<BreakdownGroup>? groups)
        => groups is { Count: > 0 } ? groups.Max(g => g.Total) : 1;



}

