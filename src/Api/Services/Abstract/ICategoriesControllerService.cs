using Api.Dtos;
using LanguageExt;

namespace Api.Services.Abstract;

public interface ICategoriesControllerService
{
    Task<Option<CategoryDto>> Get(Guid id, CancellationToken cancellationToken);
}