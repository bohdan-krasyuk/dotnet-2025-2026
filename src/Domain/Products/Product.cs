using Domain.Categories;

namespace Domain.Products;

public class Product
{
    public ProductId Id { get; }
    public string Title { get; private set; }
    public string Description { get; private set; }

    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; private set; }

    public ICollection<CategoryProduct>? Categories { get; private set; } = [];

    private Product(ProductId id, string title, string description, DateTime createdAt, DateTime? updatedAt)
    {
        Id = id;
        Title = title;
        Description = description;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static Product New(
        ProductId id,
        string title,
        string description,
        ICollection<CategoryProduct> categories)
        => new(id, title, description, DateTime.UtcNow, null)
        {
            Categories = categories
        };

    public void UpdateDetails(string title, string description)
    {
        Title = title;
        Description = description;

        UpdatedAt = DateTime.UtcNow;
    }
}