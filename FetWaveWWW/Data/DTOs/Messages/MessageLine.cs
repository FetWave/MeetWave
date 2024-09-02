﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetWave.Data.DTOs.Messages
{
    public class MessageLine
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public long ThreadId { get; set; }
        [Required]
        public DateTime CreatedTS { get; set; } = DateTime.UtcNow;
        [Required]
        public string CreatedUserId { get; set; }
        public string? LineText { get; set; }

        [ForeignKey(nameof(ThreadId))]
        public virtual MessageThread Thread { get; set; }
        [ForeignKey(nameof(CreatedUserId))]
        public virtual IdentityUser Author { get; set; }

        public virtual ICollection<MessageRead> Reads { get; set; }

    }
}
