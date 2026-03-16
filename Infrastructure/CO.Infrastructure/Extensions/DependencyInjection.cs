using CO.Application.Contracts.Interfaces.Common;
using CO.Application.Contracts.Interfaces.Services;
using CO.Infrastructure.Contracts.Implementations.Caching;
using CO.Infrastructure.Contracts.Implementations.Services;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        try
        {
            ConfigureCaching(services, configuration);

            return services;
        }
        catch (Exception)
        {
            throw;
        }

    }

    private static void ConfigureCaching(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MemoryCacheEntryOptions>(options =>
        {
            options.SlidingExpiration = TimeSpan.FromMinutes(15);
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            options.Priority = CacheItemPriority.Normal;
        });

        services.AddScoped<IUserService, UserService>();
        services.AddSingleton<ICacheService, InMemoryCacheService>();
        

        services.AddMemoryCache();

    }

}

