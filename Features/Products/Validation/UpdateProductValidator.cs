using FluentValidation;
using ProductCatalogService.Features.Products.Commands;

namespace ProductCatalogService.Features.Products.Validation;

/// <summary>
/// Validator for the <see cref="UpdateProductCommand"/>.
/// </summary>
public sealed class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage("Product ID is required.")
            .NotEqual(Guid.Empty).WithMessage("Product ID cannot be an empty GUID.");

        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");

        RuleFor(command => command.Description)
            .MaximumLength(500).WithMessage("Product description cannot exceed 500 characters.");

        RuleFor(command => command.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
    }
}
