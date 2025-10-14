using Api.Dtos;
using Api.Modules.Errors;
using Application.Categories.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("categories")]
public class CategoriesController(ISender sender) : ControllerBase
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
}