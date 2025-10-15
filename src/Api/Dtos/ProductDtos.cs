using Domain.Products;

namespace Api.Dtos;

public record ProductDto(
    Guid Id,
    string Title,
    string Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyList<CategoryProductDto>? Categories)
{
    public static ProductDto FromDomainModel(Product product)
        => new(
            product.Id.Value,
            product.Title,
            product.Description,
            product.CreatedAt,
            product.UpdatedAt,
            product.Categories != null
                ? product.Categories.Select(CategoryProductDto.FromDomainModel).ToList()
                : []);
}

public record CreateProductDto(string Title, string Description, IReadOnlyList<Guid> Categories);

public record UpdateProductDto(Guid Id, string Title, string Description, IReadOnlyList<Guid> Categories);