using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetWave.Data.DTOs.Messages
{
    public class MessageThread
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public DateTime CreatedTS { get; set; } = DateTime.UtcNow;
        [Required]
        public string CreatedUserId { get; set; }
        public string? Subject { get; set; }

        public virtual ICollection<MessageRecipient> Recipients { get; set; }
        public virtual ICollection<MessageLine> Lines { get; set; }
        [ForeignKey(nameof(CreatedUserId))]
        public virtual IdentityUser CreatedUser { get; set; }
    }
}
