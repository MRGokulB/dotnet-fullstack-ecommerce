using DotnetEcommerce.Application.Common.Interfaces;
using DotnetEcommerce.Application.Features.Orders.Dtos;
using DotnetEcommerce.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotnetEcommerce.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IPaymentService _paymentService;
    private readonly IValidator<CreateOrderFromCartRequest> _validator;
    private readonly IConfiguration _configuration;

    public OrdersController(
        IApplicationDbContext dbContext,
        IPaymentService paymentService,
        IValidator<CreateOrderFromCartRequest> validator,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _paymentService = paymentService;
        _validator = validator;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderResponse>>> GetOrders(CancellationToken cancellationToken)
    {
        var orders = await _dbContext.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return Ok(orders.Select(OrderResponse.FromEntity));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> GetOrderById(Guid id, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(OrderResponse.FromEntity(order));
    }

    [HttpPost("checkout-cart")]
    public async Task<ActionResult<CheckoutSessionResponse>> CheckoutCart(
        [FromBody] CreateOrderFromCartRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var cart = await _dbContext.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.SessionId == request.SessionId, cancellationToken);

        if (cart == null || cart.Items.Count == 0)
        {
            return BadRequest(new { message = "Cart is empty" });
        }

        var subtotal = cart.Items.Sum(i => i.UnitPrice * i.Quantity);
        var tax = Math.Round(subtotal * 0.08m, 2);
        var shipping = subtotal > 200m ? 0m : 15m;

        var order = Order.Create(
            request.CustomerEmail,
            request.CustomerName,
            request.ShippingAddress,
            subtotal,
            tax,
            shipping);

        foreach (var cartItem in cart.Items)
        {
            var productName = cartItem.Product?.Name ?? "Product Item";
            order.AddItem(cartItem.ProductId, productName, cartItem.UnitPrice, cartItem.Quantity);
        }

        _dbContext.Orders.Add(order);
        _dbContext.CartItems.RemoveRange(cart.Items);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var successUrl = _configuration["Stripe:SuccessUrl"] ?? "http://localhost:5173/success?order_id=" + order.Id;
        var cancelUrl = _configuration["Stripe:CancelUrl"] ?? "http://localhost:5173/cancel?order_id=" + order.Id;

        var (sessionId, checkoutUrl) = await _paymentService.CreateCheckoutSessionAsync(
            order,
            successUrl,
            cancelUrl,
            cancellationToken);

        return Ok(new CheckoutSessionResponse(order.Id, sessionId, checkoutUrl));
    }
}
