using ProductCatalogService.Features.Products.Contracts;

namespace ProductCatalogService.Features.Products.Queries;

/// <summary>
/// Represents a query to get a product by its ID.
/// </summary>
public record GetProductByIdQuery(
    Guid Id
);

/// <summary>
/// Represents the result of a query to get a product.
/// </summary>
public record GetProductQueryResult(
    Product? Product
);
