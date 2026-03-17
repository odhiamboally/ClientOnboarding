using CO.UI.Blazor.Contracts.Implementations.Services;
using CO.UI.Blazor.Contracts.Interfaces.Services;

namespace CO.UI.Blazor.Extensions;


public static class DependencyInjection
{
    public static IServiceCollection AddClientServices(this IServiceCollection services, IConfiguration configuration)
    {
        try
        {
            services.AddScoped<IAppNotificationService, AppNotificationService>();

            return services;
        }
        catch (Exception)
        {
            throw;
        }

    }
}
    


