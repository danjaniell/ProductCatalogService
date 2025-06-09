namespace ProductCatalogService.Features.Products.Commands;

/// <summary>
/// Represents a command to update an existing product.
/// </summary>
public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price
);
