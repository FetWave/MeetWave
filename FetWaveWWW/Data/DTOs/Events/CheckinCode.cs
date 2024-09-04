using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetWave.Data.DTOs.Events
{
    public class CheckinCode
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public int RsvpId { get; set; }
        [Required]
        public string Code { get; set; }

        [ForeignKey(nameof(RsvpId))]
        public virtual EventRSVP Rsvp { get; set; }
    }
}
