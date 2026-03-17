using CO.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Persistence.Extensions;

public static class PersistenceExtensions
{
    public static async Task ApplyMigrationsAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DBContext>();
        await context.Database.MigrateAsync();
    }
}
