using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetWave.Data.DTOs.Messages
{
    public class MessageRead
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public long MessageRecipientId { get; set; }
        [Required]
        public long MessageLineId { get; set; }
        [Required]
        public DateTime ReadTS { get; set; } = DateTime.UtcNow;


        [ForeignKey(nameof(MessageRecipientId))]
        public virtual MessageRecipient Recipient { get; set; }
        [ForeignKey(nameof(MessageLineId))]
        public virtual MessageLine Line { get; set; }
    }
}
