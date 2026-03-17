using CO.Application.Behaviours;
using CO.Application.Contracts.Implementations.Common;
using CO.Application.Contracts.Implementations.Services;
using CO.Application.Contracts.Interfaces.Common;
using CO.Application.Contracts.Interfaces.Services;
using CO.Application.Mappings;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CO.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        try
        {
            RegisterApplicationServices(services);

            return services;
        }
        catch (Exception)
        {
            throw;
        }

    }

    private static void RegisterApplicationServices(IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            cfg.AddOpenBehavior(typeof(ExceptionHandlingBehavior<,>));
            cfg.AddOpenBehavior(typeof(LoggingBehaviour<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
            cfg.AddOpenBehavior(typeof(CacheInvalidationBehavior<,>));
           
        });

        services.AddScoped<ILookupService, LookupService>();
        services.AddScoped<IClientNumberGenerator, ClientNumberGenerator>();

    }




}