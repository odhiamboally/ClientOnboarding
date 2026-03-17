using CO.Domain.Contracts.Interfaces.Common;
using CO.Domain.Contracts.Interfaces.Repositories;
using CO.Persistence.Contracts.Implementations.Interfaces;
using CO.Persistence.Contracts.Implementations.Repositories;
using CO.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CO.Persistence.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
        try
        {
            AddServices(services);

            return services;

        }
        catch (Exception)
        {
            throw;
        }
    }

    public static IServiceCollection AddDBContext(this IServiceCollection services, IConfiguration configuration)
    {
        try
        {
            var ConnString = configuration.GetConnectionString("BR");
            //services.AddPooledDbContextFactory<DBContext>(options => options.UseSqlServer(ConnString));
    
            services.AddDbContext<DBContext>(options =>
            {
                options.UseSqlServer(ConnString);
                // Use NoTracking as default for read-heavy operations; apply AsTracking() explicitly for mutations
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            return services;

        }
        catch (Exception)
        {
            throw;
        }
    }


    public static IServiceCollection AddDBContextWithRetry(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("BR") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<DbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);

                sqlOptions.CommandTimeout(30);
                sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });

            // Enable sensitive data logging in development
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }

            // Configure query tracking behavior
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        return services;
    }

    

    private static IServiceCollection AddServices(this IServiceCollection services)
    {

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IStaffMemberRepository, StaffMemberRepository>();
      

        return services;
    }


}

