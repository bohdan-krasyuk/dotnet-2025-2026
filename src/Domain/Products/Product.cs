namespace Domain.Products;

public class Product
{
    public Guid Id { get; }
    public string Title { get; private set; }
    public string Description { get; private set; }

    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; private set; }

    private Product(Guid id, string title, string description, DateTime createdAt, DateTime? updatedAt)
    {
        Id = id;
        Title = title;
        Description = description;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static Product New(
        Guid id,
        string title,
        string description)
    {
        return new Product(id, title, description, DateTime.UtcNow, null);
    }

    public void UpdateDetails(string title, string description)
    {
        Title = title;
        Description = description;

        UpdatedAt = DateTime.UtcNow;
    }
}