using System.ComponentModel.DataAnnotations;

namespace FetWaveWWW.Data.DTOs.Events
{
    public class RSVPState
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }
    }
}
