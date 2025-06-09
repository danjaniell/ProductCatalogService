using FluentValidation;
using FluentValidation.AspNetCore;
using ProductCatalogService.Features.Products.Validation;
using System.Reflection;

namespace ProductCatalogService.Extensions;

/// <summary>
/// Extension methods for FluentValidation configuration.
/// </summary>
public static class FluentValidationExtensions
{
    /// <summary>
    /// Adds FluentValidation services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddFluentValidationService(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly()); // Automatically register validators
        return services;
    }
}
