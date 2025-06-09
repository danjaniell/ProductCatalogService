namespace ProductCatalogService.Features.Products.Commands;

/// <summary>
/// Represents a command to delete a product by its ID.
/// </summary>
public record DeleteProductCommand(
    Guid Id
);
