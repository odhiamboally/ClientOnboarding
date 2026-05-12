using CO.Application.Contracts.Interfaces.Common;
using CO.Application.Utilities;
using CO.Domain.Contracts.Interfaces.Common;
using CO.Domain.Enums;
using CO.Shared.Dtos.Common;
using CO.Shared.Dtos.Dashboard;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using CO.Application.Extensions;

namespace CO.Application.Features.Dashboard.Queries;

public record GetDashboardSummaryQuery(string UserId, string? RoleScope = null) : IRequest<AppResponse<DashboardSummaryResponse>>, ICachableRequest
    
{
    public string CacheGroup => "dashboard";
    public string Discriminator => CacheKeys.Discriminator(new { UserId });
    public string? CacheUserId => UserId;
    public bool IsVersioned => false;  // invalidated directly after mutations
    public TimeSpan? Expiration => TimeSpan.FromMinutes(5); 
}


internal sealed class GetDashboardSummaryQueryHandler(IUnitOfWork _unitOfWork, ILogger<GetDashboardSummaryQueryHandler> _logger)
    : IRequestHandler<GetDashboardSummaryQuery, AppResponse<DashboardSummaryResponse>>
{
    // Standard capacity per RM — configurable via IAM settings in a future iteration
    private const int MaxCapacity = 40;

    public async Task<AppResponse<DashboardSummaryResponse>> Handle(GetDashboardSummaryQuery query, CancellationToken ct)
    {

        try
        {
            var now = DateTimeOffset.UtcNow;

            var rows = await _unitOfWork.ClientRepository
                .FindAll()
                .Select(c => new Row(
                    c.Status,
                    c.SegmentType,
                    c.ClientType,
                    c.OpenedOn,
                    c.RelationshipManager != null
                        ? $"{c.RelationshipManager.FirstName} {c.RelationshipManager.LastName}"
                        : null))
                .ToListAsync(ct);

            // ── KPIs ───────────────────────────────────────────────────────────
            var total = rows.Count;
            var active = rows.Count(c => c.Status == ClientStatus.Active);
            var pendingApproval = rows.Count(c => c.Status == ClientStatus.PendingApproval);
            var draft = rows.Count(c => c.Status == ClientStatus.Draft);

            // ── Breakdowns ─────────────────────────────────────────────────────
            var bySegment = Breakdown(rows, r => r.SegmentType.ToDisplayString(), r => r.Status);
            var byClientType = Breakdown(rows, r => r.ClientType.ToDisplayString(), r => r.Status);

            // ── Aging ──────────────────────────────────────────────────────────
            var pendingRows = rows.Where(r => r.Status == ClientStatus.PendingApproval).ToList();
            var draftRows = rows.Where(r => r.Status == ClientStatus.Draft).ToList();

            var aging = new AgingData(
                BuildAgingBucket(pendingRows, now),
                BuildAgingBucket(draftRows, now));

            // ── RM workload table ──────────────────────────────────────────────
            var rmWorkload = rows
                .Where(r => r.RmName is not null)
                .GroupBy(r => r.RmName!)
                .Select(g =>
                {
                    var rmTotal = g.Count();
                    var rmActive = g.Count(r => r.Status == ClientStatus.Active);
                    var rmPending = g.Count(r => r.Status == ClientStatus.PendingApproval);
                    var rmDraft = g.Count(r => r.Status == ClientStatus.Draft);
                    var capacity = Math.Min((int)Math.Round(rmTotal * 100.0 / MaxCapacity), 150);

                    return new RmWorkloadRow(
                        g.Key,
                        BuildInitials(g.Key),
                        rmTotal,
                        rmActive,
                        rmPending,
                        rmDraft,
                        capacity);
                })
                .OrderByDescending(r => r.Total)
                .ToList();

            return AppResponse<DashboardSummaryResponse>.Success(new(
                total, active, pendingApproval, draft,
                bySegment, byClientType, aging, rmWorkload));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching dashboard summary");
            throw;
        }
    }

   

    /// <summary>
    /// Groups a row set by a label, builds per-status counts for each group.
    /// Works for any dimension: segment, client type, or sub-breakdowns within an RM.
    /// </summary>
    private static IReadOnlyList<BreakdownGroup> Breakdown<T>(
        IEnumerable<T> source, Func<T, string> labelSelector, Func<T, ClientStatus> statusSelector)
    {
        return [.. source
                .GroupBy(labelSelector)
                .Select(g =>
                {
                    var counts = g.GroupBy(statusSelector)
                                  .ToDictionary(x => x.Key, x => x.Count());

                    return new BreakdownGroup(
                        g.Key,
                        g.Count(),
                        StatusMeta
                            .Select(s => new StatusCount(
                                s.Status.ToDisplayString(),
                                counts.GetValueOrDefault(s.Status, 0),
                                s.Color))
                            .Where(s => s.Count > 0)
                            .ToList());
                })
                .OrderByDescending(g => g.Total)];
    }

    private static string BuildInitials(string fullName)
    {
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2
            ? $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant()
            : fullName[..Math.Min(2, fullName.Length)].ToUpperInvariant();
    }

    private static AgingBucket BuildAgingBucket(IReadOnlyList<Row> rows, DateTimeOffset now)
    {
        if (rows.Count == 0)
            return new AgingBucket(0, 0, 0, 0, 0, 0);

        var ages = rows.Select(r => (now - r.OpenedOn).TotalDays).ToList();

        return new AgingBucket(
            Total: rows.Count,
            AvgDays: Math.Round(ages.Average(), 1),
            Over14Days: ages.Count(d => d > 14),
            Days7To14: ages.Count(d => d >= 7 && d <= 14),
            Days3To6: ages.Count(d => d >= 3 && d < 7),
            Under3Days: ages.Count(d => d < 3));
    }

    // A concrete projected row — avoids anonymous type inference issues
    private sealed record Row(
        ClientStatus Status,
        SegmentType SegmentType,
        ClientType ClientType,
        DateTimeOffset OpenedOn,
        string? RmName);

    // Status display order and colors — single definition used everywhere
    private static readonly (ClientStatus Status, string Color)[] StatusMeta =
    [
        (ClientStatus.Active,          "#1D9E75"),
        (ClientStatus.PendingApproval, "#378ADD"),
        (ClientStatus.Draft,           "#EF9F27"),
        (ClientStatus.Suspended,       "#E24B4A"),
        (ClientStatus.Closed,          "#888780")
    ];

}
