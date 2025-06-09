using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using ProductCatalogService.Features.Products.Commands;
using ProductCatalogService.Features.Products.Services;
using SharedContracts.Contracts;
using System.Net;

namespace ProductCatalogService.Features.Products.Endpoints;

public class CreateProduct(IProductService productService, ILogger<CreateProduct> logger) : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("", async Task<Results<Created<ProductDto>, Conflict<string>, BadRequest<IEnumerable<FluentValidation.Results.ValidationFailure>>>> (CreateProductCommand request) =>
            {
                logger.LogInformation("Attempting to create product: {ProductName}", request.Name);

                try
                {
                    var newProduct = await productService.CreateProductAsync(request);
                    logger.LogInformation("Successfully created product with ID: {ProductId}", newProduct.Id);
                    return TypedResults.Created($"api/products/{newProduct.Id}", newProduct);
                }
                catch (InvalidOperationException ex)
                {
                    logger.LogError(ex, "Conflict during product creation: {Message}", ex.Message);
                    return TypedResults.Conflict(ex.Message);
                }
                catch (ValidationException ex)
                {
                    logger.LogError(ex, "Validation error during product creation: {Message}", ex.Message);
                    return TypedResults.BadRequest(ex.Errors);
                }
            })
            .Produces<ProductDto>((int)HttpStatusCode.Created)
            .Produces<string>((int)HttpStatusCode.Conflict)
            .Produces<IEnumerable<ValidationFailure>>((int)HttpStatusCode.BadRequest)
            .WithSummary("Create Product");
    }
}
