using CO.Application.Contracts.Interfaces.Services;
using CO.Application.Features.Clients.Queries;
using CO.Application.Features.StaffMembers.Queries;
using CO.Shared.Dtos.Client;
using CO.UI.Blazor.Features.Clients.Components;
using CO.UI.Blazor.Features.Clients.Models;
using CO.UI.Blazor.Features.Utilities;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CO.UI.Blazor.Features.Clients.Pages;

public class ClientListBase : ComponentBase
{
    [Inject] protected ISender Sender { get; set; } = default!;
    [Inject] protected IDialogService DialogService { get; set; } = default!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;
    [Inject] protected ILookupService LookupService { get; set; } = default!;
    [Inject] protected IUserService UserService { get; set; } = default!;

    protected readonly List<ClientResponse> _clients = [];
    protected List<StaffMemberResponse> _staffMembers = [];
    protected ClientSearchModel _searchModel = new();
    protected LookupBundle _lookups = new();
    protected bool _loading;
    protected bool _showFilters;
    protected bool _isLastPage = true;
    protected int _totalRecords;
    protected Guid? _nextCursor;

    protected bool HasActiveFilters =>
        !string.IsNullOrWhiteSpace(_searchModel.ClientType) ||
        !string.IsNullOrWhiteSpace(_searchModel.SegmentType) ||
        !string.IsNullOrWhiteSpace(_searchModel.Status) ||
        _searchModel.RelationshipManagerId.HasValue;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            LoadLookups();
            // Staff can load in the background via service
            _staffMembers = await LookupService.GetRelationshipManagers(UserService.UserId);
            await LoadClientsAsync(reset: true);

        }
        catch (Exception)
        {
            // ToDo: Log the exception

            Snackbar.Add("Staff member features are currently unavailable.", Severity.Error);

            // If one fails, we might want to clear the other so the user doesn't see partial data
            _clients.Clear();

            // The ErrorBoundary will still catch the 'throw' if you rethrow it
            throw;
        }
    }

    private void LoadLookups()
    {
        _lookups = new LookupBundle
        {
            ClientTypes = LookupService.GetClientTypes(),
            SegmentTypes = LookupService.GetSegmentTypes(),
            Statuses = LookupService.GetLookup<Domain.Enums.ClientStatus>()
        };
    }

    protected async Task LoadClientsAsync(bool reset = false)
    {
        _loading = true;

        try
        {
            if (reset)
            {
                _clients.Clear();
                _nextCursor = null;
                _totalRecords = 0;
            }

            // Map the search model to the query request
            var clientSearchRequest = _searchModel.ToRequest();
            var request = clientSearchRequest with { Cursor = _nextCursor };

            var result = await Sender.Send(new GetClientListQuery(request, UserService.UserId));


            if (result.Successful && result.Data is not null)
            {
                var data = result.Data;
                _clients.AddRange(data.Items);
                _nextCursor = data.NextCursor;
                _isLastPage = data.IsLastPage;

                // Only update total on first page — it doesn't change on subsequent loads
                if (reset)
                    _totalRecords = data.TotalRecords;
            }
            else
            {
                Snackbar.Add(result.Message ?? "Failed to load clients.", Severity.Error);
            }

            _loading = false;
        }
        catch (Exception)
        {

            throw;
        }

    }

    protected async Task ResetAndLoad()
    {
        _searchModel.Cursor = _nextCursor;
        await LoadClientsAsync(reset: true);
    }

    protected async Task LoadMore() => await LoadClientsAsync(reset: false);

    protected void ToggleFilters() => _showFilters = !_showFilters;

    protected async Task ClearFilters()
    {
        _searchModel.Reset();
        _showFilters = false;
        await LoadClientsAsync(reset: true);
    }

    // ── Modals ────────────────────────────────────────────────────────────────

    protected async Task OpenFormModal(ClientResponse? client, int tabIndex)
    {
        var parameters = new DialogParameters
        {
            { nameof(ClientFormModal.ExistingClient), client },
            { nameof(ClientFormModal.InitialTab), tabIndex }
        };

        var options = new DialogOptions
        {
            MaxWidth = MaxWidth.Large,
            FullWidth = true,
            CloseOnEscapeKey = true,
            BackdropClick = false
        };

        var title = client is null ? "New Client" : $"Edit — {client.CompanyName}";
        var dialog = await DialogService.ShowAsync<ClientFormModal>(title, parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false })
            await LoadClientsAsync(reset: true);
    }

    protected async Task OpenViewModal(ClientResponse client, int tabIndex)
    {
        var parameters = new DialogParameters
        {
            { nameof(ClientViewModal.Client), client },
            { nameof(ClientViewModal.InitialTab), tabIndex }
        };

        var options = new DialogOptions
        {
            MaxWidth = MaxWidth.Large,
            FullWidth = true,
            CloseOnEscapeKey = true
        };

        await DialogService.ShowAsync<ClientViewModal>(client.CompanyName, parameters, options);
    }

    protected async Task OpenDeleteModal(ClientResponse client)
    {
        var parameters = new DialogParameters
        {
            { nameof(DeleteConfirmModal.ClientName), client.CompanyName },
            { nameof(DeleteConfirmModal.ClientId), client.Id }
        };

        var dialog = await DialogService.ShowAsync<DeleteConfirmModal>(
            "Confirm Delete", parameters,
            new DialogOptions { MaxWidth = MaxWidth.ExtraSmall, FullWidth = true });

        var result = await dialog.Result;
        if (result is { Canceled: false })
        {
            Snackbar.Add($"{client.CompanyName} deleted.", Severity.Success);
            await LoadClientsAsync(reset: true);
        }
    }

    protected static Color GetStatusColor(string status) => status switch
    {
        "Active" => Color.Success,
        "Draft" => Color.Warning,
        "PendingApproval" => Color.Info,
        "Suspended" => Color.Error,
        "Closed" => Color.Dark,
        _ => Color.Default
    };
}

