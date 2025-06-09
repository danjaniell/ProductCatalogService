using FluentValidation;
using ProductCatalogService.Features.Products.Commands;

namespace ProductCatalogService.Features.Products.Validation;

/// <summary>
/// Validator for the <see cref="CreateProductCommand"/>.
/// </summary>
public sealed class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");

        RuleFor(command => command.Description)
            .MaximumLength(500).WithMessage("Product description cannot exceed 500 characters.");

        RuleFor(command => command.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
    }
}
