using Domain.Categories;

namespace Api.Dtos;

public record CategoryDto(Guid Id, string Name, DateTime Created)
{
    public static CategoryDto FromDomainModel(Category category)
        => new(category.Id.Value, category.Name, category.CreatedAt);
}

public record CreateCategoryDto(string Name);