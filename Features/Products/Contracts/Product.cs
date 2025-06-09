namespace ProductCatalogService.Features.Products.Contracts;

/// <summary>
/// Represents a product in the catalog.
/// </summary>
public record Product(
    Guid Id,
    string Name,
    string Description,
    decimal Price
);
