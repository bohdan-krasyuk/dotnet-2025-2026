using Domain.Products;

namespace Application.Common.Interfaces.Repositories;

public interface IProductRepository
{
    Task<Product> AddAsync(Product entity, CancellationToken cancellationToken);
}