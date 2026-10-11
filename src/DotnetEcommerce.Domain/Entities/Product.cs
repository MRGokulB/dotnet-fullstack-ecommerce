namespace DotnetEcommerce.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;
    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; }
    public int StockQuantity { get; private set; }
    public double Rating { get; private set; }
    public int ReviewCount { get; private set; }
    public bool IsFeatured { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Product() { }

    public static Product Create(
        Guid id,
        string name,
        string slug,
        string description,
        decimal price,
        string imageUrl,
        Guid categoryId,
        int stockQuantity,
        double rating = 4.8,
        int reviewCount = 42,
        bool isFeatured = false)
    {
        return new Product
        {
            Id = id,
            Name = name,
            Slug = slug,
            Description = description,
            Price = price,
            ImageUrl = imageUrl,
            CategoryId = categoryId,
            StockQuantity = stockQuantity,
            Rating = rating,
            ReviewCount = reviewCount,
            IsFeatured = isFeatured,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void ReduceStock(int quantity)
    {
        if (StockQuantity < quantity)
        {
            throw new InvalidOperationException($"Insufficient stock for product {Name}");
        }
        StockQuantity -= quantity;
    }
}
