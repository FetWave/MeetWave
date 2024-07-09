using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace FetWaveWWW.Data.DTOs.Events
{
    [Keyless]
    [Index(nameof(EventId), nameof(CategoryId), IsUnique = true)]
    public class EventCategory
    {
        public int EventId { get; set; }
        public int CategoryId { get; set; }

        [ForeignKey(nameof(EventId))]
        public virtual CalendarEvent CalendarEvent { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; }
    }
}
