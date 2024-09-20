using MeetWave.Data.DTOs.Events;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetWave.Data.DTOs.Payments
{
    public class EventInventory
    {
        [Key]
        public int Id { get; set; }

        public int EventId { get; set; }
        [Required]
        public DateTime CreatedTS { get; set; } = DateTime.UtcNow;
        public string? CreatedUserId { get; set; }

        public DateTime? DeleteTS { get; set; }
        public string? DeleteUserId { get; set; }

        [Required]
        public string ItemName { get; set; }
        [Required]
        public long ItemPriceCents { get; set; }
        public int? ItemAvailableCount { get; set; }
        public int? Priority { get; set; }

        [ForeignKey(nameof(EventId))]
        public virtual CalendarEvent Event { get; set; }
        public virtual ICollection<OrderLineItem>? LineItems { get; set; }
    }
}
