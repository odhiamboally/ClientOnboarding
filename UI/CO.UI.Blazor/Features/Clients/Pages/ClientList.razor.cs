using CO.Application.Contracts.Interfaces.Services;
using CO.Application.Features.Clients.Commands;
using CO.Application.Features.Clients.Queries;
using CO.Application.Features.StaffMembers.Queries;
using CO.Shared.Dtos.Client;
using CO.Shared.Dtos.Common;
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
    protected bool _isFirstPage = true;
    protected int _totalRecords;
    protected int _currentPage = 1;

    // Cursor history enables Previous navigation.
    // Each entry is the cursor that was passed to load that page.
    // Page 1  → null   (no cursor, start of set)
    // Page 2  → cursor returned by page 1
    // Page 3  → cursor returned by page 2  ...etc.
    private readonly Stack<Guid?> _cursorHistory = new();
    private Guid? _currentCursor;

    protected bool HasActiveFilters =>
        !string.IsNullOrWhiteSpace(_searchModel.GlobalSearch) ||
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
            await LoadClientsAsync();

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

    protected async Task GoToFirstPage()
    {
        _cursorHistory.Clear();
        _currentCursor = null;
        _currentPage = 1;
        await LoadClientsAsync();
    }

    protected async Task GoToPreviousPage()
    {
        if (_cursorHistory.Count > 0)
        {
            _currentCursor = _cursorHistory.Pop();
            _currentPage = Math.Max(1, _currentPage - 1);
            await LoadClientsAsync();
        }
    }

    protected async Task GoToNextPage()
    {
        if (!_isLastPage && _clients.Count > 0)
        {
            // Push the cursor we used to load the current page so we can go back.
            _cursorHistory.Push(_currentCursor);
            _currentCursor = _clients[^1].Id;  // last item on current page
            _currentPage++;
            await LoadClientsAsync();
        }
    }

    private async Task LoadClientsAsync()
    {
        _loading = true;

        try
        {
            // Dispatch to the correct query based on whether any filter is active.
            // Both queries return the same AppResponse<PagedResponse<...>> shape
            // so the handling code below is identical.
            var result = HasActiveFilters ? await SearchClientsAsync() : await ListClientsAsync();
            if (result.Successful && result.Data is not null)
            {
                var data = result.Data;
                _clients.Clear();
                _clients.AddRange(data.Items);
                _isFirstPage = data.IsFirstPage;
                _isLastPage = data.IsLastPage;
                _totalRecords = data.TotalRecords;

            }
            else
            {
                Snackbar.Add(result.Message ?? "Failed to load clients.", Severity.Error);
            }

        }
        catch (Exception)
        {

            throw;
        }
        finally
        {
            _loading = false;
            StateHasChanged();
        }

    }

    private Task<AppResponse<PagedResponse<ClientResponse, Guid>>> ListClientsAsync()
    {
        var request = new ClientListRequest { Cursor = _currentCursor, PageSize = _searchModel.PageSize };
        return Sender.Send(new GetClientListQuery(request, UserService.UserId));
    }

    private Task<AppResponse<PagedResponse<ClientResponse, Guid>>> SearchClientsAsync()
    {
        var clientSearchRequest = _searchModel.ToRequest();
        var request = clientSearchRequest with { Cursor = _currentCursor };

        return Sender.Send(new SearchClientListQuery(request, UserService.UserId));
    }

    protected async Task ApplyFilters()
    {
        ResetPaginationState();
        await LoadClientsAsync();
    }

    protected async Task ClearFilters()
    {
        _searchModel.Reset();
        _showFilters = false;
        ResetPaginationState();
        await LoadClientsAsync();
    }

    protected void ToggleFilters() => _showFilters = !_showFilters;

    private void ResetPaginationState()
    {
        _cursorHistory.Clear();
        _searchModel.Cursor = null;
        _currentCursor = null;
        _currentPage = 1;
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
        {
            ResetPaginationState();
            await LoadClientsAsync();
        }
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

        var dialogResult = await dialog.Result;
        if (dialogResult is { Canceled: false })
        {
            var result = await Sender.Send(new DeleteClientCommand(client.Id, UserService.UserId));

            if (result.Successful)
            {
                Snackbar.Add($"{client.CompanyName} deleted.", Severity.Success);
                await LoadClientsAsync();
            }
            else
            {
                Snackbar.Add(result.Message ?? "Failed to delete client.", Severity.Error);
            }
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

