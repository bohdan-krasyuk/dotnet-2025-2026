namespace Domain.Categories;

public class Category
{
    public CategoryId Id { get; }

    public string Name { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public ICollection<CategoryProduct>? Products { get; private set; } = [];

    private Category(CategoryId id, string name, DateTime createdAt)
        => (Id, Name, CreatedAt) = (id, name, createdAt);

    public static Category New(CategoryId id, string name)
        => new(id, name, DateTime.UtcNow);
}