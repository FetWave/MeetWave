using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetWave.Data.DTOs.Payments
{
    public class FeeReceipt
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime CreatedTS { get; set; } = DateTime.UtcNow;
        public string? CreatedUserId { get; set; }


        [Required]
        public int FeeId { get; set; }
        [Required]
        public string UserId { get; set; }
        public string? ReceiptId { get; set; }
        public DateTime? PaidTS { get; set; }

        [ForeignKey(nameof(FeeId))]
        public virtual CalendarEventFee Fee { get; set; }
        [ForeignKey(nameof(CreatedUserId))]
        public virtual IdentityUser? CreatedUser { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual IdentityUser User { get; set; }
    }
}
