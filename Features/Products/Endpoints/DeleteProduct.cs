using Microsoft.AspNetCore.Http.HttpResults;
using ProductCatalogService.Features.Products.Commands;
using ProductCatalogService.Features.Products.Services;
using System.Net;

namespace ProductCatalogService.Features.Products.Endpoints;

public class DeleteProduct(IProductService productService, ILogger<DeleteProduct> logger) : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("{id:guid}", async Task<Results<NoContent, NotFound<string>>> (Guid id) =>
            {
                logger.LogInformation("Attempting to delete product with ID: {ProductId}", id);
                var command = new DeleteProductCommand(id);

                try
                {
                    await productService.DeleteProductAsync(command);
                    logger.LogInformation("Successfully deleted product with ID: {ProductId}", id);
                    return TypedResults.NoContent();
                }
                catch (InvalidOperationException ex)
                {
                    logger.LogError(ex, "Not found during product deletion for ID {ProductId}.", id);
                    return TypedResults.NotFound(ex.Message);
                }
            })
            .Produces((int)HttpStatusCode.NoContent)
            .Produces<string>((int)HttpStatusCode.NotFound)
            .WithSummary("Delete Product");
    }
}
