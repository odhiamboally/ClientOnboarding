using CO.Domain.Contracts.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using CO.Domain.Contracts.Implementations.Specifications;

namespace CO.Persistence.Extensions;

public static class SpecificationExtensions
{
    public static IQueryable<T> Specify<T, TCursor>(this IQueryable<T> inputQuery, ISpecification<T, TCursor> spec) where T : class
    {
        return SpecificationEvaluator<T, TCursor>.GetQuery(inputQuery, spec);
    }
}

