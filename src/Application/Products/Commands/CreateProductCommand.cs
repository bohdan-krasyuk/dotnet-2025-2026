using Application.Common.Interfaces;
using Domain.Products;
using MediatR;

namespace Application.Products.Commands;

public record CreateProductCommand : IRequest<Product>
{
    public required string Title { get; init; }
    public required string Description { get; init; }
}
// validation (command -> validate -> handler)
public class CreateProductCommandHandler(
    IProductService productService) : IRequestHandler<CreateProductCommand, Product>
{
    public Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // check if product already exists

        var result = productService.CreateProduct(Guid.NewGuid(), request.Title, request.Description);

        return Task.FromResult(result);
    }
}