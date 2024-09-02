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
        [Required]
        public string ItemName { get; set; }
        [Required]
        public long ItemPriceCents { get; set; }


        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }
    }
}
