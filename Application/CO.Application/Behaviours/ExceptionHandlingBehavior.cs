using CO.Shared.Dtos.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Application.Behaviours;

public class ExceptionHandlingBehavior<TRequest, TResponse>(ILogger<TRequest> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest 
    : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "MediatR Pipeline caught exception for {RequestName}", typeof(TRequest).Name);

            var responseType = typeof(TResponse);

            if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(AppResponse<>))
            {
                // Instead of Activator.CreateInstance, call your static Failure method
                var failureMethod = responseType.GetMethod("Failure", new[] { typeof(string) });

                if (failureMethod != null)
                {
                    var failureResponse = failureMethod.Invoke(null, new object[] { $"Server Error: {ex.Message}" });
                    return (TResponse)failureResponse!;
                }
            }

            throw; // If we can't map it, throw so GlobalErrorBoundary catches it
        }
    }

}
