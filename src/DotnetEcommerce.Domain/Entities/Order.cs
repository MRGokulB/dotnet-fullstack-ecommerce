using DotnetEcommerce.Domain.Enums;

namespace DotnetEcommerce.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public string CustomerEmail { get; private set; } = string.Empty;
    public string CustomerName { get; private set; } = string.Empty;
    public string ShippingAddress { get; private set; } = string.Empty;
    public decimal Subtotal { get; private set; }
    public decimal Tax { get; private set; }
    public decimal ShippingFee { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Currency { get; private set; } = "usd";
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public string? StripeSessionId { get; private set; }
    public string? StripePaymentIntentId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? PaidAtUtc { get; private set; }

    public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();

    private Order() { }

    public static Order Create(
        string customerEmail,
        string customerName,
        string shippingAddress,
        decimal subtotal,
        decimal tax,
        decimal shippingFee,
        string currency = "usd")
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            CustomerEmail = customerEmail,
            CustomerName = customerName,
            ShippingAddress = shippingAddress,
            Subtotal = subtotal,
            Tax = tax,
            ShippingFee = shippingFee,
            TotalAmount = subtotal + tax + shippingFee,
            Currency = currency.ToLowerInvariant(),
            Status = OrderStatus.Pending,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void AddItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        var item = OrderItem.Create(Id, productId, productName, unitPrice, quantity);
        Items.Add(item);
    }

    public void AttachStripeSession(string sessionId)
    {
        StripeSessionId = sessionId;
    }

    public void MarkAsPaid(string paymentIntentId)
    {
        Status = OrderStatus.Paid;
        StripePaymentIntentId = paymentIntentId;
        PaidAtUtc = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        Status = OrderStatus.Cancelled;
    }
}
