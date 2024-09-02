using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MeetWave.Data.DTOs.Events;
using Microsoft.AspNetCore.Identity;

namespace MeetWave.Data.DTOs.Payments
{
    public class CalendarEventFee
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime CreatedTS { get; set; } = DateTime.UtcNow;
        [Required]
        public string CreatedUserId { get; set; }

        [Required]
        public int EventId { get; set; }
        [Required]
        public string ItemName { get; set; }
        [Required]
        public long ItemPriceCents { get; set; }


        [ForeignKey(nameof(EventId))]
        public virtual CalendarEvent Event { get; set; }
        [ForeignKey(nameof(CreatedUserId))]
        public virtual IdentityUser? CreatedUser { get; set; }
    }
}
