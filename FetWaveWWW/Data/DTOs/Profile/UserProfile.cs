using MeetWave.Data.DTOs.Events;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetWave.Data.DTOs.Profile
{
    public class UserProfile
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }

        public int? PronounsId { get; set; }
        public int? DefaultRegionId { get; set; }

        public bool PrivateProfile { get; set; } = false;
        public bool? DefaultPrivateRsvp { get; set; }
        public string? AboutMe { get; set; }


        [ForeignKey(nameof(UserId))]
        public virtual IdentityUser User { get; set; }

        [ForeignKey(nameof(PronounsId))]
        public virtual UserPronouns? Pronouns { get; set; }
        [ForeignKey(nameof(DefaultRegionId))]
        public virtual Region? Region { get; set; }
    }
}
