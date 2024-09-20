using MeetWave.Data.DTOs.Payments;

namespace MeetWave.Helper
{
    public class PaymentWrapper
    {
        public class LineItem
        {
            public LineItem() { }

            public LineItem(InventoryLineItem inventory)
            {
                if (inventory?.Inventory == null)
                    throw new ArgumentException(nameof(inventory));

                Name = inventory.Inventory.ItemName;
                UnitPriceCents = inventory.Inventory.ItemPriceCents;
                Quantity = inventory.Quantity;
            }

            public string Name { get; set; }
            public long UnitPriceCents { get; set; }
            public long Quantity { get; set; }

            public long GetTotal()
                => Quantity * UnitPriceCents;
        }

        public class PrioritizedLineItem : LineItem
        {
            public int? Priority { get; set; }
        }

        public class InventoryLineItem : PrioritizedLineItem
        {
            public InventoryLineItem() { }
            public InventoryLineItem(EventInventory inventory)
            {
                Inventory = inventory;
                InventoryId = inventory.Id;
                Priority = inventory.Priority;
                Name = inventory.ItemName;
                UnitPriceCents = inventory.ItemPriceCents;
                QuantityAvailable = inventory.ItemAvailableCount;
            }

            public EventInventory Inventory { get; set; }

            public int InventoryId { get; set; }
            public int? QuantityAvailable { get; set; }
        }
    }
}
