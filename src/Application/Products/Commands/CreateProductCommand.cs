using Application.Common.Interfaces.Repositories;
using Application.Products.Exceptions;
using Domain.Categories;
using Domain.Products;
using LanguageExt;
using MediatR;

namespace Application.Products.Commands;

public record CreateProductCommand : IRequest<Either<ProductException, Product>>
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required IReadOnlyList<Guid> Categories { get; init; }
}

public class CreateProductCommandHandler(
    IProductRepository productRepository) : IRequestHandler<CreateProductCommand, Either<ProductException, Product>>
{
    public async Task<Either<ProductException, Product>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var existingProduct = await productRepository.GetByTitleAsync(request.Title, cancellationToken);

        return await existingProduct.MatchAsync(
            p => new ProductAlreadyExistException(p.Id),
            () => CreateEntity(request, cancellationToken));
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
                Product.New(productId, request.Title, request.Description, categories),
                cancellationToken);

            return product;
        }
        catch (Exception exception)
        {
            return new UnhandledProductException(ProductId.Empty(), exception);
        }
    }
}