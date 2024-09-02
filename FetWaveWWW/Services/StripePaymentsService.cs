using MeetWave.Services.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stripe;
using Stripe.Checkout;

namespace MeetWave.Services
{
    public class StripePaymentsService : IExternalPaymentsService
    {

        public StripePaymentsService()
        {
        }

        public async Task<ChargeResponse?> ChargeEventCover(string itemName, long priceCents, long quantity, string? connectedAccount, long feePercent, string returnUrl)
        {
            try
            {
                var options = new SessionCreateOptions
                {
                    LineItems = new()
                    {
                        new()
                        {
                            PriceData = new()
                            {
                                Currency = "usd",
                                ProductData = new()
                                {
                                    Name =itemName,
                                },
                                UnitAmount = priceCents,
                            },
                            Quantity = quantity,
                        },
                    },
                    PaymentIntentData = new()
                    {
                        ApplicationFeeAmount = (priceCents * feePercent) / 100
                    },
                    Mode = "payment",
                    SuccessUrl = $"{returnUrl}?session_id={{CHECKOUT_SESSION_ID}}",
                };
                var requestOptions = new RequestOptions
                {
                    StripeAccount = connectedAccount
                ,
                };
                var service = new SessionService();
                var session = service.Create(options, requestOptions);

                return new() { ReceiptId = session.Id, ChargeUrl = session.Url };
            }
            catch (Exception ex)
            {
                var a = ex;
            }
            return null;
        }

        public async Task<ChargeResponse?> ChargeEventCover(IEnumerable<Helper.PaymentWrapper.LineItem> lineItems, string? connectedAccount, long feePercent, string returnUrl)
        {
            try
            {
                var total = lineItems.Select(li => li.Quantity * li.UnitPriceCents).Sum();
                var fee = (total * feePercent) / 100;
                var options = new SessionCreateOptions
                {
                    LineItems = lineItems.Select(li => new SessionLineItemOptions()
                    {
                        PriceData = new()
                        {
                            Currency = "usd",
                            ProductData = new()
                            {
                                Name = li.Name,
                            },
                            UnitAmount = li.UnitPriceCents,
                        },
                        Quantity = li.Quantity,
                    }).ToList(),
                    PaymentIntentData = new()
                    {
                        ApplicationFeeAmount = fee
                    },
                    Mode = "payment",
                    SuccessUrl = $"{returnUrl}?session_id={{CHECKOUT_SESSION_ID}}",
                };
                var requestOptions = new RequestOptions
                {
                    StripeAccount = connectedAccount
                ,
                };
                var service = new SessionService();
                var session = service.Create(options, requestOptions);

                return new() { ReceiptId = session.Id, ChargeUrl = session.Url };
            }
            catch (Exception ex)
            {
                var a = ex;
            }
            return null;
        }
    }
}
