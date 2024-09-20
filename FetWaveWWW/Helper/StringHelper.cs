namespace MeetWave.Helper
{
    public class StringHelper
    {
        public static string GetDisplayPriceFromCents(long? priceCents)
            => priceCents > 0 ? $"{(decimal?)priceCents / 100:0.00}" : string.Empty;
    }
}
