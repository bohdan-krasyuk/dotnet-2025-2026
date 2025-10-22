using Api.Dtos;
using Api.Services.Abstract;
using Application.Common.Interfaces.Queries;
using Domain.Categories;
using LanguageExt;

namespace Api.Services.Implementation;

public class CategoriesControllerService(ICategoryQueries categoryQueries): ICategoriesControllerService
{
    public async Task<Option<CategoryDto>> Get(Guid id, CancellationToken cancellationToken)
    {
        var entity = await categoryQueries.GetByIdAsync(new CategoryId(id), cancellationToken);

        return entity.Match(
            r => CategoryDto.FromDomainModel(r),
            () => Option<CategoryDto>.None);
    }
}