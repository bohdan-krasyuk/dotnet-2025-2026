using Application.Categories.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Categories;
using LanguageExt;
using MediatR;

namespace Application.Categories.Commands;

public record CreatedCategoryCommand : IRequest<Either<CategoryException, Category>>
{
    public required string Name { get; init; }
}

public class CreatedCategoryCommandHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<CreatedCategoryCommand, Either<CategoryException, Category>>
{
    public async Task<Either<CategoryException, Category>> Handle(
        CreatedCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var existingCategory = await categoryRepository.GetByNameAsync(request.Name, cancellationToken);

        return await existingCategory.MatchAsync(
            p => new CategoryAlreadyExistException(p.Id),
            () => CreateEntity(request, cancellationToken));
    }

    private async Task<Either<CategoryException, Category>> CreateEntity(
        CreatedCategoryCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var category = await categoryRepository.AddAsync(
                Category.New(CategoryId.New(), request.Name),
                cancellationToken);

            return category;
        }
        catch (Exception exception)
        {
            return new UnhandledCategoryException(CategoryId.Empty(), exception);
        }
    }
}