using ProductCatalogService.Features.Products.Queries;
using ProductCatalogService.Features.Products.Services;
using SharedContracts.Contracts;
using System.Net;

namespace ProductCatalogService.Features.Products.Endpoints;

public class GetProduct(IProductService productService, ILogger<GetProduct> logger) : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("{id:guid}", async (Guid id) =>
            {
                logger.LogInformation("Attempting to retrieve product with ID: {ProductId}", id);
                var query = new GetProductByIdQuery(id);
                var result = await productService.GetProductByIdAsync(query);

                if (result == null)
                {
                    logger.LogWarning("Product with ID {ProductId} not found.", id);
                    return Results.NotFound();
                }

                logger.LogInformation("Successfully retrieved product with ID: {ProductId}", id);
                return Results.Ok(result);
            })
            .Produces<ProductDto>((int)HttpStatusCode.OK)
            .Produces((int)HttpStatusCode.NotFound)
            .WithSummary("Get Product");
    }
}
