using Serilog;

namespace ProductCatalogService.Extensions;

/// <summary>
/// Extension methods for Serilog configuration.
/// </summary>
public static class SerilogExtensions
{
    /// <summary>
    /// Configures Serilog for the application.
    /// </summary>
    /// <param name="hostBuilder">The web application builder.</param>
    public static WebApplicationBuilder UseSerilog(this WebApplicationBuilder hostBuilder)
    {
        hostBuilder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console()
        );
        return hostBuilder;
    }
}
