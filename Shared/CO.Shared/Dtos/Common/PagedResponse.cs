using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Shared.Dtos.Common;

public class PagedResponse<T, TCursor>
{
    public List<T> Items { get; init; } = [];
    public int TotalRecords { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; init; }
    public bool IsFirstPage { get; init; }
    public bool IsLastPage => !HasNextPage;

    // Cursor properties (optional, for keyset pagination)
    public string? Cursor { get; set; }
    public TCursor? NextCursor { get; set; }

    public PagedResponse() { }

    public PagedResponse(List<T> items, int totalRecords, int pageNumber, int pageSize, bool isFirstPage, TCursor? nextCursor)
    {
        Items = items;
        TotalRecords = totalRecords;
        CurrentPage = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(items.Count / (double)pageSize);
        IsFirstPage = isFirstPage;
        NextCursor = nextCursor;
        HasNextPage = nextCursor is not null && !nextCursor.Equals(default(TCursor));
        
    }
}
