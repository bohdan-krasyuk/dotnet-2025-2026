using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
using Domain.Categories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data.Categories;
using Xunit;

namespace Api.Tests.Integration.Categories;

public class CategoriesControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Category _firstTestCategory = CategoriesData.FirstTestCategory();
    private readonly Category _secondTestCategory = CategoriesData.SecondTestCategory();

    private const string BaseRoute = "categories";
    private readonly string _getRoute;

    public CategoriesControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _getRoute = $"{BaseRoute}/{_firstTestCategory.Id}";
    }

    [Fact]
    public async Task ShouldGetCategoryById()
    {
        // Act
        var response = await Client.GetAsync(_getRoute);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var categoryDto = await response.ToResponseModel<CategoryDto>();
        categoryDto.Id.Should().Be(_firstTestCategory.Id.Value);
        categoryDto.Name.Should().Be(_firstTestCategory.Name);
    }

    [Fact]
    public async Task ShouldCreateCategory()
    {
        // Arrange
        var request = new CreateCategoryDto(_secondTestCategory.Name);

        // Act
        var response = await Client.PostAsJsonAsync(BaseRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var categoryDto = await response.ToResponseModel<CategoryDto>();
        var categoryId = new CategoryId(categoryDto.Id);

        var dbCategory = await Context.Categories.FirstAsync(x => x.Id.Equals(categoryId));
        dbCategory.Name.Should().Be(_secondTestCategory.Name);
    }

    [Fact]
    public async Task ShouldNotCreateCategoryBecauseNameDuplication()
    {
        // Arrange
        var request = new CreateCategoryDto(_firstTestCategory.Name);

        // Act
        var response = await Client.PostAsJsonAsync(BaseRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    public async Task InitializeAsync()
    {
        await Context.Categories.AddAsync(_firstTestCategory);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Categories.RemoveRange(Context.Categories);

        await SaveChangesAsync();
    }
}