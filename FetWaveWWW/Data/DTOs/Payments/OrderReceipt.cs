using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetWave.Data.DTOs.Payments
{
    public class OrderReceipt
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime CreatedTS { get; set; } = DateTime.UtcNow;
        public string? CreatedUserId { get; set; }


        [Required]
        public int OrderId { get; set; }
        public string? ReceiptId { get; set; }
        public DateTime? PaidTS { get; set; }

        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }
        [ForeignKey(nameof(CreatedUserId))]
        public virtual IdentityUser? CreatedUser { get; set; }
    }
}
