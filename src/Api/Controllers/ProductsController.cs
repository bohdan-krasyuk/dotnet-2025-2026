using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Products.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("products")]
[ApiController]
public class ProductsController(
    IProductQueries productQueries,
    ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetProducts(CancellationToken cancellationToken)
    {
        var products = await productQueries.GetAllAsync(cancellationToken);

        return products.Select(ProductDto.FromDomainModel).ToList();
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(
        [FromBody] CreateProductDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateProductCommand
        {
            Title = request.Title,
            Description = request.Description
        };

        var newProduct = await sender.Send(input, cancellationToken);

        return newProduct.Match<ActionResult<ProductDto>>(
            p => ProductDto.FromDomainModel(p),
            e => e.ToObjectResult());
    }

    [HttpPut]
    public async Task<ActionResult<ProductDto>> UpdateProduct(
        [FromBody] UpdateProductDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateProductCommand
        {
            ProductId = request.Id,
            Title = request.Title,
            Description = request.Description
        };

        var updatedProduct = await sender.Send(input, cancellationToken);

        return updatedProduct.Match<ActionResult<ProductDto>>(
            p => ProductDto.FromDomainModel(p),
            e => e.ToObjectResult());
    }

    [HttpDelete("{productId:guid}")]
    public async Task<ActionResult<ProductDto>> DeleteProduct(
        [FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var input = new DeleteProductCommand
        {
            ProductId = productId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<ProductDto>>(
            p => ProductDto.FromDomainModel(p),
            e => e.ToObjectResult());
    }
}