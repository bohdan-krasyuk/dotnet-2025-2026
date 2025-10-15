using Domain.Products;

namespace Application.Products.Exceptions;

public abstract class ProductException(ProductId productId, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public ProductId ProductId { get; } = productId;
}

public class ProductAlreadyExistException(ProductId productId) : ProductException(productId, $"Product already exists under id {productId}");

public class ProductNotFoundException(ProductId productId) : ProductException(productId, $"Product not found under id {productId}");

public class ProductCategoriesNotFoundException(ProductId productId, IReadOnlyList<Guid> categoryIds)
    : ProductException(productId, $"Product categories not found: {string.Join(", ", categoryIds)}");

public class UnhandledProductException(ProductId productId, Exception? innerException = null)
    : ProductException(productId, "Unexpected error occurred", innerException);