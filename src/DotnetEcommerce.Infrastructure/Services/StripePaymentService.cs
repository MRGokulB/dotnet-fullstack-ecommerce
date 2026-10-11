using DotnetEcommerce.Application.Common.Interfaces;
using DotnetEcommerce.Domain.Entities;
using DotnetEcommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;

namespace DotnetEcommerce.Infrastructure.Services;

public class StripePaymentService : IPaymentService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<StripePaymentService> _logger;

    public StripePaymentService(
        IApplicationDbContext dbContext,
        IConfiguration configuration,
        ILogger<StripePaymentService> logger)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<(string SessionId, string CheckoutUrl)> CreateCheckoutSessionAsync(
        Order order,
        string successUrl,
        string cancelUrl,
        CancellationToken cancellationToken = default)
    {
        var secretKey = _configuration["Stripe:SecretKey"] ?? string.Empty;

        if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Contains("placeholder"))
        {
            var mockSessionId = "cs_sim_" + Guid.NewGuid().ToString("N")[..16];
            order.AttachStripeSession(mockSessionId);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var simulatedRedirectUrl = successUrl.Replace("{CHECKOUT_SESSION_ID}", mockSessionId);
            if (!simulatedRedirectUrl.Contains("order_id="))
            {
                simulatedRedirectUrl += (simulatedRedirectUrl.Contains('?') ? "&" : "?") + "order_id=" + order.Id;
            }

            return (mockSessionId, simulatedRedirectUrl);
        }

        var lineItems = order.Items.Select(item => new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                UnitAmount = (long)(item.UnitPrice * 100),
                Currency = order.Currency.ToLowerInvariant(),
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = item.ProductName
                }
            },
            Quantity = item.Quantity
        }).ToList();

        if (order.Tax > 0)
        {
            lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(order.Tax * 100),
                    Currency = order.Currency.ToLowerInvariant(),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Sales Tax (Estimated)"
                    }
                },
                Quantity = 1
            });
        }

        if (order.ShippingFee > 0)
        {
            lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(order.ShippingFee * 100),
                    Currency = order.Currency.ToLowerInvariant(),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Standard Shipping"
                    }
                },
                Quantity = 1
            });
        }

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            CustomerEmail = order.CustomerEmail,
            LineItems = lineItems,
            Mode = "payment",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
            Metadata = new Dictionary<string, string>
            {
                { "OrderId", order.Id.ToString() }
            }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options, cancellationToken: cancellationToken);

        order.AttachStripeSession(session.Id);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return (session.Id, session.Url);
    }

    public async Task<bool> HandleWebhookAsync(
        string jsonPayload,
        string signatureHeader,
        CancellationToken cancellationToken = default)
    {
        var webhookSecret = _configuration["Stripe:WebhookSecret"] ?? string.Empty;
        var stripeEvent = EventUtility.ConstructEvent(jsonPayload, signatureHeader, webhookSecret);

        if (stripeEvent.Type == Events.CheckoutSessionCompleted)
        {
            if (stripeEvent.Data.Object is Session session &&
                session.Metadata != null &&
                session.Metadata.TryGetValue("OrderId", out var orderIdStr) &&
                Guid.TryParse(orderIdStr, out var orderId))
            {
                var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
                if (order != null && order.Status != OrderStatus.Paid)
                {
                    order.MarkAsPaid(session.PaymentIntentId ?? session.Id);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Order {OrderId} marked as paid via Stripe webhook", orderId);
                    return true;
                }
            }
        }
        else if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
        {
            if (stripeEvent.Data.Object is PaymentIntent paymentIntent &&
                paymentIntent.Metadata != null &&
                paymentIntent.Metadata.TryGetValue("OrderId", out var orderIdStr) &&
                Guid.TryParse(orderIdStr, out var orderId))
            {
                var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
                if (order != null)
                {
                    order.MarkAsFailed();
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    _logger.LogWarning("Order {OrderId} marked as failed via Stripe webhook", orderId);
                    return true;
                }
            }
        }

        return false;
    }
}
