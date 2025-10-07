using Application.Common.Interfaces.Repositories;
using Application.Products.Exceptions;
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
}

public class UpdateProductCommandHandler(
    IProductRepository productRepository) : IRequestHandler<UpdateProductCommand, Either<ProductException, Product>>
{
    public async Task<Either<ProductException, Product>> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var productId = new ProductId(request.ProductId);

        var product = await productRepository.GetByIdAsync(productId, cancellationToken);
        return await product.MatchAsync(
            p => CheckDuplicates(p.Id, request.Title, cancellationToken)
                .BindAsync(_ => UpdateEntity(request, p, cancellationToken)),
            () => new ProductNotFoundException(productId));
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