using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace FetWaveWWW.Data.DTOs.Events
{
    [Keyless]
    [Index(nameof(EventId), nameof(DressCodeId), IsUnique = true)]
    public class EventDressCode
    {
        public int EventId { get; set; }
        public int DressCodeId { get; set; }

        [ForeignKey(nameof(EventId))]
        public virtual CalendarEvent CalendarEvent { get; set; }
        [ForeignKey(nameof(DressCodeId))]
        public virtual DressCode DressCode { get; set; }
    }
}
