using Microsoft.Extensions.Caching.Memory;
using ProductCatalogService.Features.Products.Contracts;
using System.Collections.Concurrent;

namespace ProductCatalogService.Features.Products.Data;

/// <summary>
/// Implements IProductRepository using IMemoryCache as an in-memory data store.
/// </summary>
public sealed class ProductRepository(IMemoryCache cache, ILogger<ProductRepository> logger) : IProductRepository
{
    private const string CacheKey = "ProductItems";

    /// <summary>
    /// Retrieves a product by its ID.
    /// </summary>
    /// <param name="id">The ID of the product.</param>
    /// <returns>The product if found, otherwise null.</returns>
    public Task<Product?> GetByIdAsync(Guid id)
    {
        if (cache.TryGetValue(CacheKey, out ConcurrentDictionary<Guid, Product>? productItems))
        {
            productItems.TryGetValue(id, out var product);
            logger.LogInformation("Retrieved product with ID {ProductId}: {Found}", id, product != null);
            return Task.FromResult(product);
        }
        logger.LogWarning("Cache key {CacheKey} not found for product items.", CacheKey);
        return Task.FromResult<Product?>(null);
    }

    /// <summary>
    /// Retrieves all products.
    /// </summary>
    /// <returns>A list of all products.</returns>
    public Task<IReadOnlyList<Product>> GetAllAsync()
    {
        if (cache.TryGetValue(CacheKey, out ConcurrentDictionary<Guid, Product>? productItems))
        {
            logger.LogInformation("Retrieved all {Count} product items from cache.", productItems.Count);
            return Task.FromResult<IReadOnlyList<Product>>(productItems.Values.ToList());
        }
        logger.LogWarning("Cache key {CacheKey} not found for product items. Returning empty list.", CacheKey);
        return Task.FromResult<IReadOnlyList<Product>>(new List<Product>());
    }

    /// <summary>
    /// Adds a new product.
    /// </summary>
    /// <param name="product">The product to add.</param>
    public Task AddAsync(Product product)
    {
        var productItems = cache.GetOrCreate(CacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            return new ConcurrentDictionary<Guid, Product>();
        });

        if (productItems.TryAdd(product.Id, product))
        {
            cache.Set(CacheKey, productItems); // Re-set to ensure cache update, if cache implementation requires it.
            logger.LogInformation("Added product with ID {ProductId}.", product.Id);
        }
        else
        {
            logger.LogWarning("Failed to add product with ID {ProductId}. Key already exists.", product.Id);
            throw new InvalidOperationException($"Product with ID {product.Id} already exists.");
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="product">The product to update.</param>
    public Task UpdateAsync(Product product)
    {
        if (cache.TryGetValue(CacheKey, out ConcurrentDictionary<Guid, Product>? productItems))
        {
            if (productItems.TryUpdate(product.Id, product, productItems[product.Id]))
            {
                cache.Set(CacheKey, productItems); // Re-set to ensure cache update
                logger.LogInformation("Updated product with ID {ProductId}.", product.Id);
            }
            else
            {
                logger.LogWarning("Failed to update product with ID {ProductId}. Item not found or update conflict.", product.Id);
                throw new InvalidOperationException($"Product with ID {product.Id} not found for update or update conflict.");
            }
        }
        else
        {
            logger.LogWarning("Cache key {CacheKey} not found. Cannot update product.", CacheKey);
            throw new InvalidOperationException($"Cache for products not initialized. Cannot update product with ID {product.Id}.");
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Deletes a product by its ID.
    /// </summary>
    /// <param name="id">The ID of the product to delete.</param>
    public Task DeleteAsync(Guid id)
    {
        if (cache.TryGetValue(CacheKey, out ConcurrentDictionary<Guid, Product>? productItems))
        {
            if (productItems.TryRemove(id, out _))
            {
                cache.Set(CacheKey, productItems); // Re-set to ensure cache update
                logger.LogInformation("Deleted product with ID {ProductId}.", id);
            }
            else
            {
                logger.LogWarning("Failed to delete product with ID {ProductId}. Item not found.", id);
                throw new InvalidOperationException($"Product with ID {id} not found for deletion.");
            }
        }
        else
        {
            logger.LogWarning("Cache key {CacheKey} not found. Cannot delete product.", CacheKey);
            throw new InvalidOperationException($"Cache for products not initialized. Cannot delete product with ID {id}.");
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks if a product with the given ID exists.
    /// </summary>
    /// <param name="id">The product ID to check.</param>
    /// <returns>True if the product exists, false otherwise.</returns>
    public Task<bool> ExistsAsync(Guid id)
    {
        if (cache.TryGetValue(CacheKey, out ConcurrentDictionary<Guid, Product>? productItems))
        {
            return Task.FromResult(productItems.ContainsKey(id));
        }
        return Task.FromResult(false);
    }
}
