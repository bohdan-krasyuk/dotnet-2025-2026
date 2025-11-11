using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Products.Exceptions;
using Domain.Categories;
using Domain.Products;
using LanguageExt;
using MediatR;
using Unit = LanguageExt.Unit;

namespace Application.Products.Commands;

public record CreateProductCommand : IRequest<Either<ProductException, Product>>
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required IReadOnlyList<Guid> Categories { get; init; }
    public required ProductFeatures Features { get; init; }
}

public class CreateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IEmailSendingService emailSendingService) : IRequestHandler<CreateProductCommand, Either<ProductException, Product>>
{
    public async Task<Either<ProductException, Product>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var existingProduct = await productRepository.GetByTitleAsync(request.Title, cancellationToken);

        return await existingProduct.MatchAsync(
            p => new ProductAlreadyExistException(p.Id),
            () => CheckCategories(request.Categories, cancellationToken)
                .BindAsync(_ => CreateEntity(request, cancellationToken)
                    .BindAsync(newProduct => SendNotification(newProduct, cancellationToken))));
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

    private async Task<Either<ProductException, Product>> CreateEntity(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var productId = ProductId.New();

            var categories = request.Categories
                .Select(x => CategoryProduct.New(new CategoryId(x), productId))
                .ToArray();

            var product = await productRepository.AddAsync(
                Product.New(productId, request.Title, request.Description, categories, request.Features),
                cancellationToken);

            return product;
        }
        catch (Exception exception)
        {
            return new UnhandledProductException(ProductId.Empty(), exception);
        }
    }

    private async Task<Either<ProductException, Product>> SendNotification(
        Product product,
        CancellationToken cancellationToken)
    {
        try
        {
            await emailSendingService.SendEmailAsync(
                "manager.manager@manager.com",
                $"New product added to the system: {product.Title}, under id {product.Id}");

            return product;
        }
        catch (Exception exception)
        {
            return new UnhandledProductException(ProductId.Empty(), exception);
        }
    }
}