using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CO.Persistence.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TResult> ProjectTo<TSource, TResult>(this IQueryable<TSource> query,Expression<Func<TSource, TResult>> selector)
        where TSource : class
    {
        return query.Select(selector);
    }
}
