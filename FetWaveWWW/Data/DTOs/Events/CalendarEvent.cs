﻿using MeetWave.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetWave.Data.DTOs.Events
{
    [Index(nameof(UniqueId), IsUnique = true)]
    [Index(nameof(RegionId))]
    [Index(nameof(StartDate), nameof(EndDate))]
    public class CalendarEvent
    {
        //ID
        [Key]
        public int Id { get; set; }
        [Required]
        public Guid UniqueId { get; set; } = Guid.NewGuid();

        //CRUD
        [Required]
        public DateTime CreatedTS { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedTS { get; set; }
        public DateTime? DeletedTS { get; set; }
        [Required]
        public string? CreatedUserId { get; set; }
        public string? UpdatedUserId { get; set; }

        //Meta-data
        [Required(ErrorMessage = "Region is required to plan an event.")]
        public int? RegionId { get; set; }

        //Event Info
        [Required(ErrorMessage = "Start Time is required to plan an event.")]
        [BeforeEndDate(EndDatePropertyName = nameof(EndDate), ErrorMessage = "Start date must be before the end of the event")]
        public DateTime? StartDate { get; set; }
        [Required(ErrorMessage = "End Time is required to plan an event.")]
        public DateTime? EndDate { get; set; }
        [Required(ErrorMessage = "Event Title is required to plan an event.")]
        public string? Title { get; set; }
        [Required(ErrorMessage = "Event Description is required to plan an event.")]
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? Address { get; set; }


        [ForeignKey(nameof(RegionId))]
        public virtual Region? Region { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<DressCode> DressCodes { get; set; }

        [ForeignKey(nameof(CreatedUserId))]
        public virtual IdentityUser? CreatedUser { get; set; }
        [ForeignKey(nameof(UpdatedUserId))]
        public virtual IdentityUser? UpdatedUser { get; set; }

        public virtual ICollection<EventRSVP> RSVPs { get; set; }

    }
}
