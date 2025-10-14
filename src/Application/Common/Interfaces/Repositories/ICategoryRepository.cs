using Domain.Categories;
using LanguageExt;

namespace Application.Common.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<Category> AddAsync(Category category, CancellationToken cancellationToken);
    Task<Option<Category>> GetByNameAsync(string name, CancellationToken cancellationToken);
}