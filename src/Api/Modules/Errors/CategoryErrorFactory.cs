using Application.Categories.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class CategoryErrorFactory
{
    public static ObjectResult ToObjectResult(this CategoryException error)
    {
        return new ObjectResult(error.Message)
        {
            StatusCode = error switch
            {
                CategoryAlreadyExistException => StatusCodes.Status409Conflict,
                UnhandledCategoryException => StatusCodes.Status500InternalServerError,
                CategoryNotFoundException => StatusCodes.Status404NotFound,
                _ => throw new NotImplementedException("Category error handler does not implemented")
            }
        };
    }
}