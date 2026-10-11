using DotnetEcommerce.Domain.Entities;

namespace DotnetEcommerce.Application.Common.Interfaces;

public interface IPaymentService
{
    Task<(string SessionId, string CheckoutUrl)> CreateCheckoutSessionAsync(
        Order order,
        string successUrl,
        string cancelUrl,
        CancellationToken cancellationToken = default);

    Task<bool> HandleWebhookAsync(
        string jsonPayload,
        string signatureHeader,
        CancellationToken cancellationToken = default);
}
