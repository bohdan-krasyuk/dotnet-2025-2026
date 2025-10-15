using Application.Common.Interfaces.Repositories;
using Application.Products.Exceptions;
using Domain.Categories;
using Domain.Products;
using LanguageExt;
using MediatR;
using Unit = LanguageExt.Unit;

namespace Application.Products.Commands;

public record UpdateProductCommand : IRequest<Either<ProductException, Product>>
{
    public required Guid ProductId { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required IReadOnlyList<Guid> Categories { get; init; }
}

public class UpdateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    ICategoryProductRepository categoryProductRepository) : IRequestHandler<UpdateProductCommand, Either<ProductException, Product>>
{
    public async Task<Either<ProductException, Product>> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var categoriesIds = request.Categories.Select(x => new CategoryId(x)).ToList();
        var productId = new ProductId(request.ProductId);

        var product = await productRepository.GetByIdAsync(productId, cancellationToken);
        return await product.MatchAsync(
            p => CheckDuplicates(p.Id, request.Title, cancellationToken)
                .BindAsync(_ => CheckCategories(request.Categories, cancellationToken)
                    .BindAsync(_ => UpdateEntity(request, p, cancellationToken)
                        .BindAsync(updatedProduct => UpdateCategoriesAssignments(updatedProduct, categoriesIds, cancellationToken)))),
            () => new ProductNotFoundException(productId));
    }

    private async Task<Either<ProductException, Product>> UpdateCategoriesAssignments(
        Product product,
        IReadOnlyList<CategoryId> inputCategoryIds,
        CancellationToken cancellationToken)
    {
        try
        {
            var sourceCategories = await categoryProductRepository.GetByProductIdAsync(product.Id, cancellationToken);

            var newCategoriesIds = inputCategoryIds.Where(ic => sourceCategories.All(sc => sc.CategoryId != ic)).ToList();
            var newCategoriesProducts = newCategoriesIds.Select(c => CategoryProduct.New(c, product.Id)).ToList();
            if (newCategoriesProducts.Any())
            {
                await categoryProductRepository.AddRangeAsync(newCategoriesProducts, cancellationToken);
            }

            var unassignedCategories = sourceCategories.Where(sc => inputCategoryIds.All(ic => ic != sc.CategoryId)).ToList();
            if (unassignedCategories.Any())
            {
                await categoryProductRepository.RemoveRangeAsync(unassignedCategories, cancellationToken);
            }

            return product;
        }
        catch (Exception exception)
        {
            return new UnhandledProductException(product.Id, exception);
        }
    }

    private async Task<Either<ProductException, Unit>> CheckCategories(
        IReadOnlyList<Guid> categories,
        CancellationToken cancellationToken)
    {
        var entities = await categoryRepository.GetByIdsAsync(
            categories.Select(x => new CategoryId(x)).ToList(),
            cancellationToken);

        var missingCategories = categories.Where(x => entities.All(y => y.Id.Value != x)).ToList();

        return missingCategories.Any()
            ? new ProductCategoriesNotFoundException(ProductId.Empty(), missingCategories)
            : Unit.Default;
    }

    private async Task<Either<ProductException, Product>> UpdateEntity(
        UpdateProductCommand request,
        Product product,
        CancellationToken cancellationToken)
    {
        try
        {
            product.UpdateDetails(request.Title, request.Description);

            return await productRepository.UpdateAsync(product, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UnhandledProductException(product.Id, exception);
        }
    }

    private async Task<Either<ProductException, Unit>> CheckDuplicates(
        ProductId currentProductId,
        string title,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByTitleAsync(title, cancellationToken);

        return product.Match<Either<ProductException, Unit>>(
            p => p.Id.Equals(currentProductId) ? Unit.Default : new ProductAlreadyExistException(p.Id),
            () => Unit.Default);
    }
}