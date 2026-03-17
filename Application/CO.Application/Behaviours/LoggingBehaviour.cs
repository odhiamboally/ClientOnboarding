using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CO.Application.Behaviours;

public class LoggingBehaviour<TRequest, TResponse>
    (ILogger<LoggingBehaviour<TRequest, TResponse>> _logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Handling {RequestName}", requestName);

        var sw = Stopwatch.StartNew();
        var response = await next(ct);
        sw.Stop();

        if (sw.ElapsedMilliseconds > 500)
        {
            _logger.LogWarning(
                "Slow request detected: {RequestName} took {ElapsedMs}ms",
                requestName, sw.ElapsedMilliseconds);
        }
        else
        {
            _logger.LogInformation(
                "Handled {RequestName} in {ElapsedMs}ms",
                requestName, sw.ElapsedMilliseconds);
        }

        return response;
    }
}
