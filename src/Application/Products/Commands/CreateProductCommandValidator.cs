using FluentValidation;

namespace Application.Products.Commands;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(3).MaximumLength(255);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(3).MaximumLength(500);
        RuleFor(x => x.Categories).NotEmpty();
    }
}