using Domain.Products;

namespace Api.Dtos;

public record ProductDto(Guid Id, string Title, string Description, DateTime CreatedAt, DateTime? UpdatedAt)
{
    public static ProductDto FromDomainModel(Product product)
        => new(product.Id.Value, product.Title, product.Description, product.CreatedAt, product.UpdatedAt);
}

public record CreateProductDto(string Title, string Description);

public record UpdateProductDto(Guid Id, string Title, string Description);