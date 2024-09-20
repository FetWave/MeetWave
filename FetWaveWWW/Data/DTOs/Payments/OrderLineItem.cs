using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetWave.Data.DTOs.Payments
{
    public class OrderLineItem
    {

        [Key]
        public long Id { get; set; }
        [Required]
        public DateTime CreatedTS { get; set; } = DateTime.UtcNow;
        public string? CreatedUserId { get; set; }

        [Required]
        public int OrderId { get; set; }

        public int? InventoryId { get; set; }
        public string? ItemName { get; set; }
        public long? ItemPriceCents { get; set; }
        [Required]
        public long ItemQuantity { get; set; } = 1;


        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }
        public virtual EventInventory? Inventory { get; set; }

        public string? GetName()
            => ItemName ?? Inventory?.ItemName ?? throw new Exception("No item name defined for line item");
        public long? GetPriceCents()
            => ItemPriceCents ?? Inventory?.ItemPriceCents ?? throw new Exception("No item price defined for line item");
    }
}
