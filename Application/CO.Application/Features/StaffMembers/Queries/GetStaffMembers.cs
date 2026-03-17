using CO.Application.Contracts.Interfaces.Common;
using CO.Application.Mappings;
using CO.Application.Utilities;
using CO.Domain.Contracts.Interfaces.Common;
using CO.Shared.Dtos.Client;
using CO.Shared.Dtos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Application.Features.StaffMembers.Queries;

// ── Get Active Staff Members (for RM dropdown) ────────────────────────────────

public record GetStaffMembersQuery(string UserId) : IRequest<AppResponse<List<StaffMemberResponse>>>, ICachableRequest
{
    public string CacheGroup => "staff-members";
    public string CacheDiscriminator => "all";           // no filter — one entry per user
    public string? CacheUserId => UserId;
    public bool IsVersioned => false;
}


internal sealed class GetStaffMembersQueryHandler(IUnitOfWork _unitOfWork, ILogger<GetStaffMembersQueryHandler> _logger)
    : IRequestHandler<GetStaffMembersQuery, AppResponse<List<StaffMemberResponse>>>
{
    public async Task<AppResponse<List<StaffMemberResponse>>> Handle(GetStaffMembersQuery query, CancellationToken ct)
    {
        try
        {
            var staff = await _unitOfWork.StaffMemberRepository.FindAll().ToListAsync(cancellationToken: ct);
            var mapped = staff.Select(s => s.ToStaffMemberResponse()).ToList();
            return AppResponse<List<StaffMemberResponse>>.Success($"Success", mapped);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching staff members");
            throw;
        }
    }
}

