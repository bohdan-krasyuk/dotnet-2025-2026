using Application.Products.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class ProductErrorFactory
{
    public static ObjectResult ToObjectResult(this ProductException error)
    {
        return new ObjectResult(error.Message)
        {
            StatusCode = error switch
            {
                ProductAlreadyExistException => StatusCodes.Status409Conflict,
                UnhandledProductException => StatusCodes.Status500InternalServerError,
                ProductNotFoundException or ProductCategoriesNotFoundException => StatusCodes.Status404NotFound,
                _ => throw new NotImplementedException("Product error handler does not implemented")
            }
        };
    }
}