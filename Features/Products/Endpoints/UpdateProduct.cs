using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using ProductCatalogService.Features.Products.Commands;
using ProductCatalogService.Features.Products.Services;
using SharedContracts.Contracts;
using System.Net;

namespace ProductCatalogService.Features.Products.Endpoints;

public class UpdateProduct(IProductService productService, ILogger<UpdateProduct> logger) : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("", async Task<Results<Ok<ProductDto>, NotFound<string>, BadRequest<IEnumerable<FluentValidation.Results.ValidationFailure>>, BadRequest<string>>> (UpdateProductCommand request) =>
            {
                logger.LogInformation("Attempting to update product with ID: {ProductId}", request.Id);

                try
                {
                    var updatedProduct = await productService.UpdateProductAsync(request);
                    logger.LogInformation("Successfully updated product with ID: {ProductId}", updatedProduct.Id);
                    return TypedResults.Ok(updatedProduct);
                }
                catch (InvalidOperationException ex)
                {
                    logger.LogError(ex, "Not found during product update for ID {ProductId}.", request.Id);
                    return TypedResults.NotFound(ex.Message);
                }
                catch (ValidationException ex)
                {
                    logger.LogError(ex, "Validation error during product update: {Message}", ex.Message);
                    return TypedResults.BadRequest(ex.Errors);
                }
            })
            .Produces<ProductDto>((int)HttpStatusCode.OK)
            .Produces<string>((int)HttpStatusCode.NotFound)
            .Produces<IEnumerable<ValidationFailure>>((int)HttpStatusCode.BadRequest)
            .WithSummary("Update Product");
    }
}
