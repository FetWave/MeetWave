using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetWave.Data.DTOs.Events
{
    public class EventRSVP
    {
        [Key]
        public int Id { get; set; }

        public Guid UniqueId { get; set; } = Guid.NewGuid();

        //CRUD
        [Required]
        public DateTime CreatedTS { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedTS { get; set; }
        public DateTime? DeletedTS { get; set; }
        [Required]
        public string? CreatedUserId { get; set; }
        public string? UpdatedUserId { get; set; }
        public string? ApprovedByUserId { get; set; }

        public int? EventId { get; set; }
        public string? UserId { get; set; }
        public int? RSVPStateId { get; set; }
        public bool? Private { get; set; } = false;
        public DateTime? ApprovedTS { get; set; }

        [ForeignKey(nameof(EventId))]
        public virtual CalendarEvent Event { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual IdentityUser User { get; set; }
        [ForeignKey(nameof(RSVPStateId))]
        public virtual RSVPState State { get; set; }

        [ForeignKey(nameof(CreatedUserId))]
        public virtual IdentityUser? CreatedUser { get; set; }
        [ForeignKey(nameof(UpdatedUserId))]
        public virtual IdentityUser? UpdatedUser { get; set; }
        [ForeignKey(nameof(ApprovedByUserId))]
        public virtual IdentityUser? ApprovedByUser { get; set; }
    }
}
