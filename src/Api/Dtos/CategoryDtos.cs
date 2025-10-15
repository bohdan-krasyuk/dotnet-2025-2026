using Domain.Categories;

namespace Api.Dtos;

public record CategoryDto(Guid Id, string Name, DateTime Created)
{
    public static CategoryDto FromDomainModel(Category category)
        => new(category.Id.Value, category.Name, category.CreatedAt);
}

public record CreateCategoryDto(string Name);

public record CategoryProductDto(CategoryDto? Category)
{
    public static CategoryProductDto FromDomainModel(CategoryProduct product)
        => new(product.Category == null ? null : CategoryDto.FromDomainModel(product.Category));
}