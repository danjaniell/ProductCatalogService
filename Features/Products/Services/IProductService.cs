using ProductCatalogService.Features.Products.Commands;
using ProductCatalogService.Features.Products.Queries;
using SharedContracts.Contracts;

namespace ProductCatalogService.Features.Products.Services;

/// <summary>
/// Defines the contract for the product catalog business logic service.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Retrieves a product by its ID.
    /// </summary>
    /// <param name="query">The query containing the product ID.</param>
    /// <returns>A DTO representing the product, or null if not found.</returns>
    Task<ProductDto?> GetProductByIdAsync(GetProductByIdQuery query);

    /// <summary>
    /// Retrieves all products.
    /// </summary>
    /// <param name="query">The query to get all products.</param>
    /// <returns>A list of DTOs representing all products.</returns>
    Task<IReadOnlyList<ProductDto>> GetAllProductsAsync(GetAllProductsQuery query);

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="command">The command to create the product.</param>
    /// <returns>A DTO representing the newly created product.</returns>
    Task<ProductDto> CreateProductAsync(CreateProductCommand command);

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="command">The command to update the product.</param>
    /// <returns>A DTO representing the updated product.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the product does not exist.</exception>
    Task<ProductDto> UpdateProductAsync(UpdateProductCommand command);

    /// <summary>
    /// Deletes a product by its ID.
    /// </summary>
    /// <param name="command">The command to delete the product.</param>
    /// <exception cref="InvalidOperationException">Thrown if the product does not exist.</exception>
    Task DeleteProductAsync(DeleteProductCommand command);
}
