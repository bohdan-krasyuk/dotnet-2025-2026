using Domain.Products;

namespace Application.Common.Interfaces.Repositories;

public interface IProductImageRepository
{
    Task<ProductImage> AddAsync(ProductImage entity, CancellationToken cancellationToken);
}