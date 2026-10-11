using DotnetEcommerce.Application.Common.Interfaces;
using DotnetEcommerce.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace DotnetEcommerce.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(
        IApplicationDbContext dbContext,
        IPaymentService paymentService,
        ILogger<PaymentsController> logger)
    {
        _dbContext = dbContext;
        _paymentService = paymentService;
        _logger = logger;
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> HandleWebhook(CancellationToken cancellationToken)
    {
        string json;
        using (var reader = new StreamReader(HttpContext.Request.Body))
        {
            json = await reader.ReadToEndAsync(cancellationToken);
        }

        var signatureHeader = Request.Headers["Stripe-Signature"].ToString();

        try
        {
            var processed = await _paymentService.HandleWebhookAsync(json, signatureHeader, cancellationToken);
            return Ok(new { received = true, processed });
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe webhook signature validation failed");
            return BadRequest(new { message = "Invalid webhook signature" });
        }
    }

    [HttpPost("simulate-success/{orderId:guid}")]
    public async Task<IActionResult> SimulateSuccess(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        if (order == null)
        {
            return NotFound(new { message = "Order not found" });
        }

        if (order.Status == OrderStatus.Paid)
        {
            return Ok(new { message = "Order is already paid", orderId = order.Id, status = "Paid" });
        }

        var mockPaymentIntent = "pi_ecom_" + Guid.NewGuid().ToString("N")[..16];
        order.MarkAsPaid(mockPaymentIntent);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new {
            message = "Simulated webhook event processed successfully",
            orderId = order.Id,
            status = "Paid",
            paymentIntentId = mockPaymentIntent
        });
    }
}
