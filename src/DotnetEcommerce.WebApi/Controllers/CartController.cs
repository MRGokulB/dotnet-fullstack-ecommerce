using DotnetEcommerce.Application.Common.Interfaces;
using DotnetEcommerce.Application.Features.Cart.Dtos;
using DotnetEcommerce.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotnetEcommerce.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IValidator<AddToCartRequest> _validator;

    public CartController(
        IApplicationDbContext dbContext,
        IValidator<AddToCartRequest> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    [HttpGet("{sessionId}")]
    public async Task<ActionResult<CartResponse>> GetCart(string sessionId, CancellationToken cancellationToken)
    {
        var cart = await _dbContext.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.SessionId == sessionId, cancellationToken);

        if (cart == null)
        {
            cart = Cart.Create(sessionId);
            _dbContext.Carts.Add(cart);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return Ok(CartResponse.FromEntity(cart));
    }

    [HttpPost("add")]
    public async Task<ActionResult<CartResponse>> AddToCart(
        [FromBody] AddToCartRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var product = await _dbContext.Products.FindAsync(new object[] { request.ProductId }, cancellationToken);
        if (product == null)
        {
            return NotFound(new { message = "Product not found" });
        }

        var cart = await _dbContext.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.SessionId == request.SessionId, cancellationToken);

        if (cart == null)
        {
            cart = Cart.Create(request.SessionId);
            _dbContext.Carts.Add(cart);
        }

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + request.Quantity);
        }
        else
        {
            var newItem = CartItem.Create(cart.Id, product.Id, request.Quantity, product.Price);
            cart.Items.Add(newItem);
        }

        cart.Touch();
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(CartResponse.FromEntity(cart));
    }

    [HttpPut("items/{itemId:guid}")]
    public async Task<ActionResult<CartResponse>> UpdateItemQuantity(
        Guid itemId,
        [FromBody] UpdateCartItemRequest request,
        CancellationToken cancellationToken)
    {
        var item = await _dbContext.CartItems
            .Include(i => i.Cart)
            .FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);

        if (item == null)
        {
            return NotFound();
        }

        if (request.Quantity <= 0)
        {
            _dbContext.CartItems.Remove(item);
        }
        else
        {
            item.UpdateQuantity(request.Quantity);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        var updatedCart = await _dbContext.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstAsync(c => c.Id == item.CartId, cancellationToken);

        return Ok(CartResponse.FromEntity(updatedCart));
    }

    [HttpDelete("items/{itemId:guid}")]
    public async Task<ActionResult<CartResponse>> RemoveItem(Guid itemId, CancellationToken cancellationToken)
    {
        var item = await _dbContext.CartItems
            .Include(i => i.Cart)
            .FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);

        if (item == null)
        {
            return NotFound();
        }

        var cartId = item.CartId;
        _dbContext.CartItems.Remove(item);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var updatedCart = await _dbContext.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstAsync(c => c.Id == cartId, cancellationToken);

        return Ok(CartResponse.FromEntity(updatedCart));
    }

    [HttpDelete("{sessionId}")]
    public async Task<IActionResult> ClearCart(string sessionId, CancellationToken cancellationToken)
    {
        var cart = await _dbContext.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.SessionId == sessionId, cancellationToken);

        if (cart != null)
        {
            _dbContext.CartItems.RemoveRange(cart.Items);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return Ok(new { cleared = true });
    }
}
