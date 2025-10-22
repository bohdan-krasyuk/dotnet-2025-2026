using Domain.Categories;
using Domain.Products;

namespace Tests.Data.Categories;

public static class CategoriesData
{
    public static Category FirstTestCategory() => Category.New(CategoryId.New(), "First test category");
    public static Category SecondTestCategory() => Category.New(CategoryId.New(), "Second test category");

    public static CategoryProduct FirstTestCategoryProduct(CategoryId categoryId, ProductId productId)
        => CategoryProduct.New(categoryId, productId);
}