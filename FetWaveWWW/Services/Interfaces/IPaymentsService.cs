namespace MeetWave.Services.Interfaces
{
    public interface IPaymentsService
    {
        Task<string?> ChargeEventCover(string itemName, long priceCents, long quantity, string? connectedAccount, long feePercent, string returnUrl);
    }
}
