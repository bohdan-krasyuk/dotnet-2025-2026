using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Settings;
using Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository, IProductQueries
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context, ApplicationSettings settings)
    {
        var connectionString = settings.ConnectionStrings.DefaultConnection; // example of usage

        _context = context;
    }

    public async Task<Product> AddAsync(Product entity, CancellationToken cancellationToken)
    {
        await _context.Products.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Products.ToListAsync(cancellationToken);
    }
}