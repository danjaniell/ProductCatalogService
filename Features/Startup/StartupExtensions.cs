using Microsoft.Extensions.DependencyInjection.Extensions;
using ProductCatalogService.Features.Products.Data;
using ProductCatalogService.Features.Products.Endpoints;
using ProductCatalogService.Features.Products.Services;
using System.Reflection;

namespace ProductCatalogService.Features.Startup;

/// <summary>
/// Extension methods for configuring product catalog-related services.
/// </summary>
public static class StartupExtensions
{
    /// <summary>
    /// Adds product catalog specific services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    public static IServiceCollection AddProductCatalogServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();

        return services;
    }

    /// <summary>
    /// Registers all types in the specified assembly that implement the <see cref="IEndpoint"/> interface as transient
    /// services in the provided <see cref="IServiceCollection"/>.
    /// </summary>
    /// <remarks>This method scans the provided assembly for non-abstract, non-interface types that implement
    /// the <see cref="IEndpoint"/> interface and registers them as transient services. It ensures that multiple
    /// implementations of <see cref="IEndpoint"/> can coexist in the service collection.</remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the services will be added.</param>
    /// <param name="assembly">The assembly to scan for types implementing <see cref="IEndpoint"/>.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddEndpoints(
    this IServiceCollection services,
    Assembly assembly)
    {
        ServiceDescriptor[] serviceDescriptors = assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    /// <summary>
    /// Maps all registered endpoints to the specified route builder.
    /// </summary>
    /// <remarks>This method retrieves all registered <see cref="IEndpoint"/> instances from the application's
    /// service container  and maps them to the provided route builder. Endpoints are mapped using their <see
    /// cref="IEndpoint.MapEndpoint"/> method.</remarks>
    /// <param name="app">The <see cref="WebApplication"/> instance used to retrieve services and configure routing.</param>
    /// <param name="routeGroupBuilder">An optional <see cref="RouteGroupBuilder"/> to use for mapping endpoints.  If <paramref
    /// name="routeGroupBuilder"/> is <see langword="null"/>, the <paramref name="app"/> instance is used as the route
    /// builder.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> instance for further configuration.</returns>
    public static IApplicationBuilder MapEndpoints(
    this WebApplication app,
    RouteGroupBuilder? routeGroupBuilder = null)
    {
        using (var scope = app.Services.CreateScope())
        {
            IEnumerable<IEndpoint> endpoints = scope.ServiceProvider
                .GetRequiredService<IEnumerable<IEndpoint>>();

            IEndpointRouteBuilder builder =
                routeGroupBuilder is null ? app : routeGroupBuilder;

            foreach (IEndpoint endpoint in endpoints)
            {
                endpoint.MapEndpoint(builder);
            }
        }

        return app;
    }
}
