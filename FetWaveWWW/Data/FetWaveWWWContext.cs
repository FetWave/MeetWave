using FetWaveWWW.Data.DTOs.Events;
using FetWaveWWW.Data.DTOs.Messages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection.Emit;

namespace FetWaveWWW.Data;

public class FetWaveWWWContext : IdentityDbContext<IdentityUser>
{
    public FetWaveWWWContext(DbContextOptions<FetWaveWWWContext> options)
        : base(options) { }

    public DbSet<CalendarEvent> Events { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<DressCode> DressCodes { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<EventRSVP> RSVPs { get; set; }
    public DbSet<RSVPState> RSVPStates { get; set; }

    public DbSet<MessageThread> MessageThreads { get; set; }
    public DbSet<MessageLine> MessageLines { get; set; }
    public DbSet<MessageRecipient> MessageRecipients { get; set; }
    public DbSet<MessageRead> MessageReads { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.Entity<CalendarEvent>()
        .HasMany(e => e.DressCodes)
        .WithMany(e => e.Events);

        builder.Entity<CalendarEvent>()
        .HasMany(e => e.Categories)
        .WithMany(e => e.Events);
    }
}
