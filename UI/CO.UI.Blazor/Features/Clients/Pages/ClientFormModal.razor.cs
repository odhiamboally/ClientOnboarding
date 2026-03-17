using CO.Application.Contracts.Interfaces.Services;
using CO.Application.Features.Clients.Commands;
using CO.Application.Features.StaffMembers.Queries;
using CO.Shared.Dtos.Client;
using CO.Shared.Validation.Validators.Clients;
using CO.UI.Blazor.Features.Clients.Models;
using CO.UI.Blazor.Features.Clients.Validators;
using CO.UI.Blazor.Features.Utilities;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CO.UI.Blazor.Features.Clients.Pages;

public class ClientFormModalBase : ComponentBase
{
    [CascadingParameter] protected IMudDialogInstance MudDialog { get; set; } = default!;
    [Inject] protected ISender Sender { get; set; } = default!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;
    [Inject] protected ILookupService LookupService { get; set; } = default!;
    [Inject] protected IUserService UserService { get; set; } = default!;

    [Parameter] public ClientResponse? ExistingClient { get; set; }
    [Parameter] public int InitialTab { get; set; }


    protected MudForm _form = default!;
    protected LookupBundle _lookups = new();
    protected ClientFormModel _model = new();
    protected List<StaffMemberResponse> _staffMembers = [];
    protected ClientFormValidator _validator = new();
    protected bool _loadingClient;
    protected bool _saving;
    protected int _activeTab;

    protected override async Task OnInitializedAsync()
    {
        LoadLookups();

        _staffMembers = await LookupService.GetRelationshipManagers(UserService.UserId);

        _activeTab = InitialTab;

        if (ExistingClient is not null)
            _model = ClientFormModel.FromResponse(ExistingClient);
    }

    private void LoadLookups()
    {
        _lookups = new LookupBundle
        {
            ClientTypes = LookupService.GetClientTypes(),
            SegmentTypes = LookupService.GetSegmentTypes(),
            SubSegmentTypes = LookupService.GetSubSegmentTypes(),
            LinesOfBusiness = LookupService.GetLinesOfBusiness(),
            IdentificationTypes = LookupService.GetIdentificationTypes()
        };
    }

    protected async Task HandleSubmit()
    {
        try
        {
            if (ExistingClient is null)
            {
                var result = await Sender.Send(
                    new CreateClientCommand(_model.ToCreateRequest(), UserService.UserId));

                if (result.Successful)
                {
                    Snackbar.Add(result.Message ?? "Client created.", Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    Snackbar.Add(result.Message ?? "Failed to create client.", Severity.Error);
                }
            }
            else
            {
                var result = await Sender.Send(
                    new UpdateClientCommand(
                        ExistingClient.Id,
                        _model.ToUpdateRequest(ExistingClient.Id),
                        UserService.UserId));

                if (result.Successful)
                {
                    Snackbar.Add("Client updated successfully.", Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    Snackbar.Add(result.Message ?? "Failed to update client.", Severity.Error);
                }
            }
        }
        catch (Exception)
        {
            // ToDo: Log the exception details for debugging purposes
            Snackbar.Add("A technical error occurred on the server. Please try again.", Severity.Error);
            throw;
        }
        finally
        {
            _saving = false;
            StateHasChanged();
        }

    }

    protected void Cancel() => MudDialog.Cancel();
}

