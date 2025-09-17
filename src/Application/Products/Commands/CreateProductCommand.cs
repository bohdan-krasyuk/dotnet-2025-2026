using Application.Common.Interfaces.Repositories;
using Domain.Products;
using MediatR;

namespace Application.Products.Commands;

public record CreateProductCommand : IRequest<Product>
{
    public required string Title { get; init; }
    public required string Description { get; init; }
}

public class CreateProductCommandHandler(
    IProductRepository productRepository) : IRequestHandler<CreateProductCommand, Product>
{
    public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.AddAsync(
            Product.New(Guid.NewGuid(), request.Title, request.Description),
            cancellationToken);

        return product;
    }
}