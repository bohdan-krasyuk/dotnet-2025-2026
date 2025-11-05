using Application.Common.Interfaces.Repositories;
using Domain.Products;

namespace Infrastructure.Persistence.Repositories;

public class ProductImageRepository(ApplicationDbContext context) : IProductImageRepository
{
    public async Task<ProductImage> AddAsync(ProductImage entity, CancellationToken cancellationToken)
    {
        await context.ProductImages.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return entity;
    }
}