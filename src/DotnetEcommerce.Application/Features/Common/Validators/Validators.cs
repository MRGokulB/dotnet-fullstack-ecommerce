using DotnetEcommerce.Application.Features.Cart.Dtos;
using DotnetEcommerce.Application.Features.Orders.Dtos;
using FluentValidation;

namespace DotnetEcommerce.Application.Features.Common.Validators;

public class AddToCartRequestValidator : AbstractValidator<AddToCartRequest>
{
    public AddToCartRequestValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.SessionId).NotEmpty();
    }
}

public class CreateOrderFromCartRequestValidator : AbstractValidator<CreateOrderFromCartRequest>
{
    public CreateOrderFromCartRequestValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
        RuleFor(x => x.CustomerEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ShippingAddress).NotEmpty().MaximumLength(300);
    }
}
