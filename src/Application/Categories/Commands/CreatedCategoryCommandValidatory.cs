using FluentValidation;

namespace Application.Categories.Commands;

public class CreatedCategoryCommandValidatory : AbstractValidator<CreatedCategoryCommand>
{
    public CreatedCategoryCommandValidatory()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
    }
}