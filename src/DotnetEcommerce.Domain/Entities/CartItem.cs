namespace DotnetEcommerce.Domain.Entities;

public class CartItem
{
    public Guid Id { get; private set; }
    public Guid CartId { get; private set; }
    public Cart? Cart { get; private set; }
    public Guid ProductId { get; private set; }
    public Product? Product { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    private CartItem() { }

    public static CartItem Create(Guid cartId, Guid productId, int quantity, decimal unitPrice)
    {
        return new CartItem
        {
            Id = Guid.NewGuid(),
            CartId = cartId,
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }

    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
    }
}
