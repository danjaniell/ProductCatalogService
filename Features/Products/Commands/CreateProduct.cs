using ProductCatalogService.Features.Products.Contracts;

namespace ProductCatalogService.Features.Products.Commands;

/// <summary>
/// Represents a command to create a new product.
/// </summary>
public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price
)
{
    /// <summary>
    /// Converts the command to a Product record, generating a new ID.
    /// </summary>
    public Product ToProduct() => new (Guid.NewGuid(), Name, Description, Price);
}
