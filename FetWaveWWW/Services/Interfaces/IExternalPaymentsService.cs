namespace MeetWave.Services.Interfaces
{

    public class ChargeResponse
    {
        public string ChargeUrl { get; set; }
        public string ReceiptId { get; set; }
    }

    public interface IExternalPaymentsService
    {
        Task<ChargeResponse?> ChargeEventCover(string itemName, long priceCents, long quantity, string? connectedAccount, long feePercent, string returnUrl);
        Task<ChargeResponse?> ChargeEventCover(IEnumerable<Helper.PaymentWrapper.LineItem> lineItems, string? connectedAccount, long feePercent, string returnUrl);
    }
}
