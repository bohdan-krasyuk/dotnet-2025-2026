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

public record ProductFeaturesDto(
    double? Weight,
    double? Height,
    double? Width,
    double? Ram,
    double? Storage,
    double? ScreenSize)
{
    public static ProductFeatures ToDomainModel(ProductFeaturesDto? dto)
        => dto == null
            ? ProductFeatures.Empty()
            : ProductFeatures.New(
                dto.Weight,
                dto.Height,
                dto.Width,
                dto.Ram,
                dto.Storage,
                dto.ScreenSize);
}

public record CreateProductDto(string Title, string Description, IReadOnlyList<Guid> Categories, ProductFeaturesDto? Features);

public record UpdateProductDto(Guid Id, string Title, string Description, IReadOnlyList<Guid> Categories, ProductFeaturesDto? Features);