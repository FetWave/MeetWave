namespace MeetWave.Helper
{
    public class PaymentWrapper
    {
        public class LineItem
        {
            public string Name { get; set; }
            public long UnitPriceCents { get; set; }
            public long Quantity { get; set; }
        }
    }
}
