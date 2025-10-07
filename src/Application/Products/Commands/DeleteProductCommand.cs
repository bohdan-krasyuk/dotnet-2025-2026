using Application.Common.Interfaces.Repositories;
using Application.Products.Exceptions;
using Domain.Products;
using LanguageExt;
using MediatR;

namespace Application.Products.Commands;

public record DeleteProductCommand : IRequest<Either<ProductException, Product>>
{
    public required Guid ProductId { get; init; }
}

public class DeleteProductCommandHandler(
    IProductRepository productRepository) : IRequestHandler<DeleteProductCommand, Either<ProductException, Product>>
{
    public async Task<Either<ProductException, Product>> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var productId = new ProductId(request.ProductId);
        var product = await productRepository.GetByIdAsync(productId, cancellationToken);
        return await product.MatchAsync(
            p => DeleteEntity(p, cancellationToken),
            () => new ProductNotFoundException(productId));
    }

    private async Task<Either<ProductException, Product>> DeleteEntity(
        Product product,
        CancellationToken cancellationToken)
    {
        try
        {
            return await productRepository.DeleteAsync(product, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UnhandledProductException(product.Id, exception);
        }
    }
}