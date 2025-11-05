using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Products.Exceptions;
using Domain.Products;
using LanguageExt;
using MediatR;

namespace Application.Products.Commands;

public record UploadProductImageCommand : IRequest<Either<ProductException, Product>>
{
    public required Guid ProductId { get; init; }
    public required string OriginalName { get; init; }
    public required Stream FileStream { get; init; }
}

public class UploadProductImageCommandHandler(
    IProductRepository productRepository,
    IProductImageRepository productImageRepository,
    IFileStorage fileStorage)
    : IRequestHandler<UploadProductImageCommand, Either<ProductException, Product>>
{
    public async Task<Either<ProductException, Product>> Handle(
        UploadProductImageCommand request,
        CancellationToken cancellationToken)
    {
        var productId = new ProductId(request.ProductId);
        var product = await productRepository.GetByIdAsync(productId, cancellationToken);
        return await product.MatchAsync(
            p => CreateEntity(request, p, cancellationToken)
                .BindAsync(i => UploadImage(request.FileStream, i, p, cancellationToken)),
            () => new ProductNotFoundException(productId));
    }

    private async Task<Either<ProductException, Product>> UploadImage(
        Stream fileStream,
        ProductImage productImage,
        Product product,
        CancellationToken cancellationToken)
    {
        try
        {
            await fileStorage.UploadAsync(fileStream, productImage.GetFilePath(), cancellationToken);

            return product;
        }
        catch (Exception exception)
        {
            return new UnhandledProductException(product.Id, exception);
        }
    }

    private async Task<Either<ProductException, ProductImage>> CreateEntity(
        UploadProductImageCommand request,
        Product product,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = ProductImage.New(product.Id, request.OriginalName);
            return await productImageRepository.AddAsync(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UnhandledProductException(product.Id, exception);
        }
    }
}