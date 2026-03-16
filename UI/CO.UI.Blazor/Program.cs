using CO.Application.Extensions;
using CO.Infrastructure.Extensions;
using CO.Persistence.Extensions;
using CO.Shared.Validation.Extensions;
using CO.UI.Blazor.Extensions;
using CO.UI.Blazor.Middleware;
using CO.UI.Blazor.Shared.Components;
using Microsoft.AspNetCore.Diagnostics;
using MudBlazor;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.ShowTransitionDuration = 200;
    config.SnackbarConfiguration.HideTransitionDuration = 200;
});

// Layer registrations — order matters: Persistence before Application
builder.Services.AddSharedValidationServices();
builder.Services.AddPersistenceServices();
builder.Services.AddDBContext(builder.Configuration);          
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddClientServices(builder.Configuration);   

builder.Services.AddScoped<ExceptionHandler>();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

try
{
    await app.Services.ApplyMigrationsAsync();
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred while applying database migrations.");
    throw;
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(new ExceptionHandlerOptions
    {
        ExceptionHandler = async context =>
        {
            var exceptionHandler = context.RequestServices.GetRequiredService<ExceptionHandler>();
            await exceptionHandler.TryHandleAsync(
                context,
                context.Features.Get<IExceptionHandlerFeature>()?.Error!,
                CancellationToken.None);
        }
    });

    app.UseHsts();

}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
