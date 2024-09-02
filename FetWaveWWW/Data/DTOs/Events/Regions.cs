﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MeetWave.Data.DTOs.Events
{
    [Index(nameof(StateCode))]
    public class Region
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? StateCode { get; set; }
        [Required]
        public string? Name { get; set; }
    }
}
