using ProductCatalogService.Features.Products.Contracts;

namespace ProductCatalogService.Features.Products.Queries;

/// <summary>
/// Represents a query to get all products.
/// </summary>
public record GetAllProductsQuery();

/// <summary>
/// Represents the result of a query to get all products.
/// </summary>
public record GetAllProductsQueryResult(
    IReadOnlyList<Product> Products
);
