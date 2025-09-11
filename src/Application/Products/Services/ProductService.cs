using Application.Common.Interfaces;
using Domain.Products;

namespace Application.Products.Services;

public class ProductService : IProductService
{
    private readonly List<Product> _products = [];

    public Product CreateProduct(Guid id, string title, string description)
    {
        var product = Product.New(id, title, description);

        _products.Add(product);

        return product;
    }

    public IReadOnlyList<Product> GetProducts()
    {
        return _products;
    }
}