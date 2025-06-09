using Bogus;
using Microsoft.Extensions.Caching.Memory;
using ProductCatalogService.Features.Products.Contracts;
using System.Collections.Concurrent;

namespace ProductCatalogService.Extensions;

/// <summary>
/// Extension methods for IMemoryCache and fake data generation.
/// </summary>
public static class MemoryCacheExtensions
{
    private const string CacheKey = "ProductItems";

    /// <summary>
    /// Adds IMemoryCache to the service collection and configures it with fake data initialization.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddMemoryCacheWithFakes(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<IHostedService, CacheInitializationService>();
        return services;
    }

    /// <summary>
    /// Initializes the IMemoryCache with fake inventory data using Bogus.
    /// </summary>
    /// <param name="cache">The IMemoryCache instance.</param>
    /// <summary>
    /// Hosted service that initializes the cache with fake data when the application starts.
    /// </summary>
    public class CacheInitializationService(IMemoryCache cache, ILogger<CacheInitializationService> logger) : IHostedService
    {
        private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        private readonly ILogger<CacheInitializationService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Initializing cache with fake data...");

                // Generate fake data
                var productFaker = new Faker<Product>()
                    .CustomInstantiator(f => new Product(Guid.NewGuid(), f.Commerce.ProductName(), f.Commerce.ProductDescription(), f.Finance.Amount(10, 1000, 2)))
                    .RuleFor(p => p.Id, f => Guid.NewGuid())
                    .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                    .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                    .RuleFor(p => p.Price, f => f.Finance.Amount(10, 1000, 2));

                var items = new ConcurrentDictionary<Guid, Product>();
                for (int i = 0; i < 50; i++)
                {
                    var product = productFaker.Generate();
                    if (!items.TryAdd(product.Id, product))
                    {
                        _logger.LogWarning("Duplicate ProductId generated: {ProductId}", product.Id);
                    }
                }

                // Set cache with new data
                _cache.Set(
                    CacheKey,
                    items,
                    new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) }
                );

                _logger.LogInformation("Successfully initialized cache with {Count} inventory items", items.Count);

                // Verify the data was stored
                if (_cache.TryGetValue(CacheKey, out ConcurrentDictionary<Guid, Product>? cachedItems))
                {
                    _logger.LogDebug("Cache verification: {Count} items found in cache", cachedItems?.Count ?? 0);
                }
                else
                {
                    _logger.LogError("Failed to verify cache data after initialization");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing cache with fake data");
                throw;
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
