namespace DotnetEcommerce.Domain.Entities;

public class Category
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Icon { get; private set; } = string.Empty;

    public ICollection<Product> Products { get; private set; } = new List<Product>();

    private Category() { }

    public static Category Create(Guid id, string name, string slug, string description, string icon)
    {
        return new Category
        {
            Id = id,
            Name = name,
            Slug = slug,
            Description = description,
            Icon = icon
        };
    }
}
