namespace MeetWave.Services.Interfaces
{
    public interface IPaymentsService
    {
        Task<string?> ChargeEventCover(long priceCents, long quantity, string? connectedAccount, long feePercent, string returnUrl);
    }
}
