using MeetWave.Services.Interfaces;
using Stripe;
using Stripe.Checkout;

namespace MeetWave.Services
{
    public class StripePaymentsService : IPaymentsService
    {

        public StripePaymentsService()
        {
        }

        public async Task<ReceiptDetails?> GetReceiptDetails(string receiptId)
        {
            var service = new SessionService();
            var receipt = await service.GetAsync(receiptId);
            var status = receipt.PaymentStatus.ToLower() switch
            {
                "no_payment_required" => ReceiptDetails.PaidStatus.NoPaymentNeeded,
                "paid" => ReceiptDetails.PaidStatus.Paid,
                "unpaid" => ReceiptDetails.PaidStatus.Unpaid,
                _ => (ReceiptDetails.PaidStatus?)null
            };
            return new()
            {
                ReceiptId = receiptId,
                Status = status,
            };
        }

        public async Task<ChargeResponse?> ChargeEventCover(string itemName, long priceCents, long quantity, string? connectedAccount, long feePercent, string returnUrl)
        {
            try
            {
                var fee = (long?)null;
                if (!string.IsNullOrWhiteSpace(connectedAccount) && feePercent > 0)
                {
                    var total = priceCents * quantity;
                    fee = (total * feePercent) / 100;
                }

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

        public async Task<ChargeResponse?> ChargeEventCover(IEnumerable<Helper.PaymentWrapper.LineItem> lineItems, string? connectedAccount, long feePercent, string returnUrl)
        {
            try
            {
                var fee = (long?)null;
                if (!string.IsNullOrWhiteSpace(connectedAccount) && feePercent > 0)
                {
                    var total = lineItems.Select(li => li.Quantity * li.UnitPriceCents).Sum();
                    fee = (total * feePercent) / 100;
                }
                
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
