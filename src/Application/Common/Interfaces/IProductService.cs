using Domain.Products;

namespace Application.Common.Interfaces;

public interface IProductService
{
    Product CreateProduct(Guid id, string title, string description);
    IReadOnlyList<Product> GetProducts();
}