using MeetWave.Services.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stripe;
using Stripe.Checkout;

namespace MeetWave.Services
{
    public class StripePaymentsService : IPaymentsService
    {

        public StripePaymentsService(SessionService sessionService)
        {
        }

        public async Task<string?> ChargeEventCover(long priceCents, long quantity, string? connectedAccount, long feePercent, string returnUrl)
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
                                    Name = "Event Cover",
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

                return session.Url;
            }
            catch (Exception ex)
            {
                var a = ex;
            }
            return null;
        }
    }
}
