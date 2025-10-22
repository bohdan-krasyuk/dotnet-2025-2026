using Domain.Categories;
using Domain.Products;

namespace Tests.Data.Products;

public static class ProductsData
{
    public static Product FirstTestProduct()
        => Product.New(ProductId.New(), "First test product", "First test product description", []);

    public static Product SecondTestProduct()
        => Product.New(ProductId.New(), "Second test product", "Second test product description", []);
}