using Api.Dtos;
using Api.Modules.Errors;
using Api.Services.Abstract;
using Application.Categories.Commands;
using Application.Common.Interfaces.Queries;
using Domain.Categories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("categories")]
public class CategoriesController(
    ISender sender,
    ICategoriesControllerService controllerService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(
        [FromBody] CreateCategoryDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreatedCategoryCommand
        {
            Name = request.Name
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<CategoryDto>>(
            r => CategoryDto.FromDomainModel(r),
            e => e.ToObjectResult());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> Get(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var entity = await controllerService.Get(id, cancellationToken);

        return entity.Match<ActionResult<CategoryDto>>(
            e => e,
            () => NotFound());
    }
}