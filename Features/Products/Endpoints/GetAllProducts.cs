using ProductCatalogService.Features.Products.Queries;
using ProductCatalogService.Features.Products.Services;
using SharedContracts.Contracts;
using System.Net;

namespace ProductCatalogService.Features.Products.Endpoints;

public class GetAllProducts(IProductService productService, ILogger<GetAllProducts> logger) : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("", async () =>
            {
                logger.LogInformation("Attempting to retrieve all products.");
                var query = new GetAllProductsQuery();
                var result = await productService.GetAllProductsAsync(query);

                logger.LogInformation("Successfully retrieved {Count} products.", result.Count);
                return Results.Ok(result);
            })
            .Produces((int)HttpStatusCode.OK, typeof(IEnumerable<ProductDto>))
            .WithSummary("Get All Products");
    }
}
