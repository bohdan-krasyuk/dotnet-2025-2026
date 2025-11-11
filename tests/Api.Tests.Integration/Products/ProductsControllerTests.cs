using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
using Domain.Categories;
using Domain.Products;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data.Categories;
using Tests.Data.Products;
using Xunit;

namespace Api.Tests.Integration.Products;

public class ProductsControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Category _firstTestCategory = CategoriesData.FirstTestCategory();
    private readonly Category _secondTestCategory = CategoriesData.SecondTestCategory();

    private readonly Product _firstTestProduct = ProductsData.FirstTestProduct();
    private readonly Product _secondTestProduct = ProductsData.SecondTestProduct();

    private readonly CategoryProduct _firstTestCategoryProduct;

    private const string BaseRoute = "products";

    public ProductsControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _firstTestCategoryProduct = CategoriesData.FirstTestCategoryProduct(_firstTestCategory.Id, _firstTestProduct.Id);
    }

    [Fact]
    public async Task ShouldCreateProduct()
    {
        // Arrange
        var request = new CreateProductDto(
            Title: _secondTestProduct.Title,
            Description: _secondTestProduct.Description,
            Categories: [_firstTestCategory.Id.Value],
            Features: new ProductFeaturesDto(
                Weight: 50,
                Height: 100,
                Width: 200,
                Ram: null,
                Storage: null,
                ScreenSize: null));

        // Act
        var response = await Client.PostAsJsonAsync(BaseRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldNotUpdateProductBecauseCategoryDoesNotExist()
    {
        // Arrange
        var request = new UpdateProductDto(
            Id: _firstTestProduct.Id.Value,
            Title: _secondTestProduct.Title,
            Description: _secondTestProduct.Description,
            Categories: [Guid.NewGuid()],
            Features: null);

        // Act
        var response = await Client.PutAsJsonAsync(BaseRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldUpdateProduct()
    {
        // Arrange
        var request = new UpdateProductDto(
            Id: _firstTestProduct.Id.Value,
            Title: _secondTestProduct.Title,
            Description: _secondTestProduct.Description,
            Categories: [_secondTestCategory.Id.Value],
            Features: new ProductFeaturesDto(
                Weight: 50,
                Height: 100,
                Width: 200,
                Ram: null,
                Storage: null,
                ScreenSize: null));

        // Act
        var response = await Client.PutAsJsonAsync(BaseRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var updatedProductDto = await response.ToResponseModel<ProductDto>();
        var productId = new ProductId(updatedProductDto.Id);

        var productDb = await Context.Products.AsNoTracking().FirstAsync(x => x.Id.Equals(productId));
        productDb.Title.Should().Be(_secondTestProduct.Title);
        productDb.Description.Should().Be(_secondTestProduct.Description);

        var assignedCategories = await Context.CategoryProducts
            .AsNoTracking()
            .Where(x => x.ProductId.Equals(productId))
            .ToListAsync();

        assignedCategories.Should().HaveCount(1);
        assignedCategories.First().CategoryId.Should().Be(_secondTestCategory.Id);
    }

    public async Task InitializeAsync()
    {
        await Context.Categories.AddAsync(_firstTestCategory);
        await Context.Categories.AddAsync(_secondTestCategory);
        await Context.Products.AddAsync(_firstTestProduct);
        await Context.CategoryProducts.AddAsync(_firstTestCategoryProduct);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Categories.RemoveRange(Context.Categories);
        Context.Products.RemoveRange(Context.Products);
        Context.CategoryProducts.RemoveRange(Context.CategoryProducts);

        await SaveChangesAsync();
    }
}