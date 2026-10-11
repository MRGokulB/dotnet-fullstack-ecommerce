using DotnetEcommerce.Domain.Entities;

namespace DotnetEcommerce.Application.Features.Cart.Dtos;

public record AddToCartRequest(
    Guid ProductId,
    int Quantity,
    string SessionId
);

public record UpdateCartItemRequest(
    int Quantity
);

public record CartItemResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string? ProductImageUrl,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal
)
{
    public static CartItemResponse FromEntity(CartItem i) =>
        new(
            i.Id,
            i.ProductId,
            i.Product?.Name ?? string.Empty,
            i.Product?.ImageUrl,
            i.UnitPrice,
            i.Quantity,
            i.UnitPrice * i.Quantity
        );
}

public record CartResponse(
    Guid Id,
    string SessionId,
    int TotalItems,
    decimal Subtotal,
    IEnumerable<CartItemResponse> Items
)
{
    public static CartResponse FromEntity(DotnetEcommerce.Domain.Entities.Cart cart)
    {
        var items = cart.Items.Select(CartItemResponse.FromEntity).ToList();
        return new CartResponse(
            cart.Id,
            cart.SessionId,
            items.Sum(i => i.Quantity),
            items.Sum(i => i.LineTotal),
            items
        );
    }
}
