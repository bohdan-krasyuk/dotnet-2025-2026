namespace Domain.Products;

public class ProductImage
{
    public ProductImageId Id { get; }
    public string OriginalName { get; }

    public ProductId ProductId { get; }

    private ProductImage(ProductImageId id, string originalName, ProductId productId)
    {
        Id = id;
        OriginalName = originalName;
        ProductId = productId;
    }

    public static ProductImage New(ProductId productId, string originalName)
    {
        return new ProductImage(ProductImageId.New(), originalName, productId);
    }

    public string GetFilePath()
        => $"{ProductId}/{Id}{Path.GetExtension(OriginalName)}";
}