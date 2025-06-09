using ProductCatalogService.Features.Products.Commands;
using ProductCatalogService.Features.Products.Data;
using ProductCatalogService.Features.Products.Queries;
using SharedContracts.Contracts;

namespace ProductCatalogService.Features.Products.Services;

/// <summary>
/// Implements the IProductService, containing the business logic for product catalog management.
/// </summary>
public sealed class ProductService(IProductRepository productRepository, ILogger<ProductService> logger) : IProductService
{
    /// <summary>
    /// Retrieves a product by its ID.
    /// </summary>
    /// <param name="query">The query containing the product ID.</param>
    /// <returns>A DTO representing the product, or null if not found.</returns>
    public async Task<ProductDto?> GetProductByIdAsync(GetProductByIdQuery query)
    {
        var product = await productRepository.GetByIdAsync(query.Id);
        if (product == null)
        {
            logger.LogWarning("Product with ID {ProductId} not found.", query.Id);
            return null;
        }
        return new ProductDto(product.Id, product.Name, product.Description, product.Price);
    }

    /// <summary>
    /// Retrieves all products.
    /// </summary>
    /// <param name="query">The query to get all products.</param>
    /// <returns>A list of DTOs representing all products.</returns>
    public async Task<IReadOnlyList<ProductDto>> GetAllProductsAsync(GetAllProductsQuery query)
    {
        var products = await productRepository.GetAllAsync();
        return products.Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price)).ToList();
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="command">The command to create the product.</param>
    /// <returns>A DTO representing the newly created product.</returns>
    public async Task<ProductDto> CreateProductAsync(CreateProductCommand command)
    {
        var newProduct = command.ToProduct();
        await productRepository.AddAsync(newProduct); // Repository handles existence check
        logger.LogInformation("Created product with ID {ProductId}.", newProduct.Id);
        return new ProductDto(newProduct.Id, newProduct.Name, newProduct.Description, newProduct.Price);
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="command">The command to update the product.</param>
    /// <returns>A DTO representing the updated product.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the product does not exist.</exception>
    public async Task<ProductDto> UpdateProductAsync(UpdateProductCommand command)
    {
        var existingProduct = await productRepository.GetByIdAsync(command.Id);
        if (existingProduct == null)
        {
            logger.LogError("Attempted to update product with ID {ProductId} that does not exist.", command.Id);
            throw new InvalidOperationException($"Product with ID {command.Id} does not exist.");
        }

        var updatedProduct = existingProduct with
        {
            Name = command.Name,
            Description = command.Description,
            Price = command.Price
        };
        await productRepository.UpdateAsync(updatedProduct);
        logger.LogInformation("Updated product with ID {ProductId}.", updatedProduct.Id);
        return new ProductDto(updatedProduct.Id, updatedProduct.Name, updatedProduct.Description, updatedProduct.Price);
    }

    /// <summary>
    /// Deletes a product by its ID.
    /// </summary>
    /// <param name="command">The command to delete the product.</param>
    /// <exception cref="InvalidOperationException">Thrown if the product does not exist.</exception>
    public async Task DeleteProductAsync(DeleteProductCommand command)
    {
        var existingProduct = await productRepository.GetByIdAsync(command.Id);
        if (existingProduct == null)
        {
            logger.LogError("Attempted to delete product with ID {ProductId} that does not exist.", command.Id);
            throw new InvalidOperationException($"Product with ID {command.Id} does not exist.");
        }

        await productRepository.DeleteAsync(command.Id);
        logger.LogInformation("Deleted product with ID {ProductId}.", command.Id);
    }
}
