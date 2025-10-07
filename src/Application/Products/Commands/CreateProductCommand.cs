using Application.Common.Interfaces.Repositories;
using Application.Products.Exceptions;
using Domain.Products;
using LanguageExt;
using MediatR;

namespace Application.Products.Commands;

public record CreateProductCommand : IRequest<Either<ProductException, Product>>
{
    public required string Title { get; init; }
    public required string Description { get; init; }
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
            var product = await productRepository.AddAsync(
                Product.New(ProductId.New(), request.Title, request.Description),
                cancellationToken);

            return product;
        }
        catch (Exception exception)
        {
            return new UnhandledProductException(ProductId.Empty(), exception);
        }
    }
}