using ProductCatalogService.Features.Products.Contracts;

namespace ProductCatalogService.Features.Products.Data;

/// <summary>
/// Defines the contract for a product data repository.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Retrieves a product by its ID.
    /// </summary>
    /// <param name="id">The ID of the product.</param>
    /// <returns>The product if found, otherwise null.</returns>
    Task<Product?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all products.
    /// </summary>
    /// <returns>A list of all products.</returns>
    Task<IReadOnlyList<Product>> GetAllAsync();

    /// <summary>
    /// Adds a new product.
    /// </summary>
    /// <param name="product">The product to add.</param>
    Task AddAsync(Product product);

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="product">The product to update.</param>
    Task UpdateAsync(Product product);

    /// <summary>
    /// Deletes a product by its ID.
    /// </summary>
    /// <param name="id">The ID of the product to delete.</param>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Checks if a product with the given ID exists.
    /// </summary>
    /// <param name="id">The product ID to check.</param>
    /// <returns>True if the product exists, false otherwise.</returns>
    Task<bool> ExistsAsync(Guid id);
}
