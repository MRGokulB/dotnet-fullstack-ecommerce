namespace DotnetEcommerce.Domain.Entities;

public class Cart
{
    public Guid Id { get; private set; }
    public string SessionId { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public ICollection<CartItem> Items { get; private set; } = new List<CartItem>();

    private Cart() { }

    public static Cart Create(string sessionId)
    {
        return new Cart
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
    }

    public void Touch()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
