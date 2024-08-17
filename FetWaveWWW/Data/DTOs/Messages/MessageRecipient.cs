using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FetWaveWWW.Data.DTOs.Messages
{
    public class MessageRecipient
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public long ThreadId { get; set; }
        [Required]
        public DateTime AddedTS { get; set; } = DateTime.UtcNow;
        [Required]
        public string AddedByUserId { get; set; }
        public DateTime RemovedTS { get; set; }
        public string RemovedByUserId { get; set; }
        [Required]
        public string RecipientUserId { get; set; }


        [ForeignKey(nameof(ThreadId))]
        public virtual MessageThread Thread { get; set; }
        [ForeignKey(nameof(RecipientUserId))]
        public virtual IdentityUser Recipient { get; set; }
        [ForeignKey(nameof(AddedByUserId))]
        public virtual IdentityUser AddedUser { get; set; }
        [ForeignKey(nameof(RemovedByUserId))]
        public virtual IdentityUser RemovedUser { get; set; }

        public virtual ICollection<MessageRead> Reads { get; set; }
    }
}
