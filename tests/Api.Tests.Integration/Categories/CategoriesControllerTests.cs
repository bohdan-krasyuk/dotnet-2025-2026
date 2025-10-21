using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
using Domain.Categories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Xunit;

namespace Api.Tests.Integration.Categories;

public class CategoriesControllerTests(IntegrationTestWebFactory factory)
    : BaseIntegrationTest(factory), IAsyncLifetime
{
    [Fact]
    public async Task ShouldCreateCategory()
    {
        // Arrange
        var request = new CreateCategoryDto("Test Category 1");

        // Act
        var response = await Client.PostAsJsonAsync("categories", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var categoryDto = await response.ToResponseModel<CategoryDto>();
        var categoryId = new CategoryId(categoryDto.Id);

        var dbCategory = await Context.Categories.FirstAsync(x => x.Id.Equals(categoryId));
        dbCategory.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task ShouldNotCreateCategoryBecauseNameDuplication()
    {
        // Arrange
        var request = new CreateCategoryDto("First test category");

        // Act
        var response = await Client.PostAsJsonAsync("categories", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    public async Task InitializeAsync()
    {
        var category = Category.New(CategoryId.New(), "First test category");
        await Context.Categories.AddAsync(category);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    public async Task DisposeAsync()
    {
        Context.Categories.RemoveRange(Context.Categories);

        await Context.SaveChangesAsync();
    }
}