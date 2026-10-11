using DotnetEcommerce.Domain.Entities;

namespace DotnetEcommerce.Application.Features.Orders.Dtos;

public record CreateOrderFromCartRequest(
    string SessionId,
    string CustomerEmail,
    string CustomerName,
    string ShippingAddress
);

public record OrderItemResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice
)
{
    public static OrderItemResponse FromEntity(OrderItem i) =>
        new(i.Id, i.ProductId, i.ProductName, i.UnitPrice, i.Quantity, i.TotalPrice);
}

public record OrderResponse(
    Guid Id,
    string CustomerEmail,
    string CustomerName,
    string ShippingAddress,
    decimal Subtotal,
    decimal Tax,
    decimal ShippingFee,
    decimal TotalAmount,
    string Currency,
    string Status,
    string? StripeSessionId,
    DateTime CreatedAtUtc,
    DateTime? PaidAtUtc,
    IEnumerable<OrderItemResponse> Items
)
{
    public static OrderResponse FromEntity(Order o) =>
        new(
            o.Id,
            o.CustomerEmail,
            o.CustomerName,
            o.ShippingAddress,
            o.Subtotal,
            o.Tax,
            o.ShippingFee,
            o.TotalAmount,
            o.Currency,
            o.Status.ToString(),
            o.StripeSessionId,
            o.CreatedAtUtc,
            o.PaidAtUtc,
            o.Items.Select(OrderItemResponse.FromEntity).ToList()
        );
}

public record CheckoutSessionResponse(
    Guid OrderId,
    string SessionId,
    string CheckoutUrl
);
