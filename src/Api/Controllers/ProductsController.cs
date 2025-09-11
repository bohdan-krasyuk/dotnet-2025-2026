using Api.Dtos;
using Application.Common.Interfaces;
using Application.Products.Commands;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("products")]
[ApiController]
public class ProductsController(
    IProductService productService,
    IValidator<CreateProductDto> createProductDtoValidator,
    ISender sender) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<ProductDto>> GetProducts(CancellationToken cancellationToken)
    {
        var products = productService.GetProducts();

        return products.Select(ProductDto.FromDomainModel).ToList();
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(
        [FromBody] CreateProductDto request,
        CancellationToken cancellationToken)
    {
        var validationResult = createProductDtoValidator.Validate(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var input = new CreateProductCommand
        {
            Title = request.Title,
            Description = request.Description
        };

        var newProduct = await sender.Send(input, cancellationToken);

        return ProductDto.FromDomainModel(newProduct);
    }
}