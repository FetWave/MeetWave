using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FetWaveWWW.Data.DTOs.Profile
{
    public class UserPronouns
    {
        [Key]
        public int Id { get; set; }
        public DateTime? IsPublicTS { get; set; }
        public string? PublicApproverUserId { get; set; }
        [Required]
        public DateTime CreatedTS { get; set; } = DateTime.UtcNow;
        public string? CreatedUserId { get; set; }
        public DateTime? DeletedTS { get; set; }
        public string? DeletedUserId { get; set; }

        [Required]
        public string Value { get; set; }


        [ForeignKey(nameof(PublicApproverUserId))]
        public virtual IdentityUser? ApproverUser { get; set; }
        [ForeignKey(nameof(CreatedUserId))]
        public virtual IdentityUser? CreatedUser { get; set; }

        [ForeignKey(nameof(DeletedUserId))]
        public virtual IdentityUser? DeletedUser { get; set; }
    }
}
