using System.ComponentModel.DataAnnotations;

namespace FetWaveWWW.Data.DTOs.Messages
{
    public class MessageThread
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public DateTime CreatedTS { get; set; } = DateTime.UtcNow;
        [Required]
        public string CreatedUserId { get; set; }

        public virtual ICollection<MessageRecipient> Recipients { get; set; }
        public virtual ICollection<MessageLine> Lines { get; set; }
    }
}
