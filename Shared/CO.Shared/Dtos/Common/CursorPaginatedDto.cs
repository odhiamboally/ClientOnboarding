namespace CO.Shared.Dtos.Common;

/// <summary>
/// Wraps a cursor-paginated result set.  Using a dedicated DTO keeps the
/// pagination contract separate from domain entities and reduces the data
/// returned to callers to exactly what they need.
/// </summary>
public record CursorPaginatedDto<T>(
    IReadOnlyList<T> Items,
    int TotalRecords,
    bool HasNextPage,
    bool IsFirstPage,
    Guid? NextCursor);
