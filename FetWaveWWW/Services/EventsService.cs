﻿using FetWaveWWW.Data;
using FetWaveWWW.Data.DTOs.Events;
using Google.Apis.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Caching.Memory;
using Radzen;

namespace FetWaveWWW.Services
{
    public class EventsService
    {
        private readonly IMemoryCache _cache;
        private readonly FetWaveWWWContext _context;

        public EventsService(IMemoryCache cache, FetWaveWWWContext context)
        {
            _cache = cache;
            _context = context;
        }

        private async Task<IList<Region>?> GetCachedRegions()
            => await _cache.GetOrCreateAsync("EventRegions", async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(12);
                return await _context.Regions
                    .AsNoTracking()
                    .ToListAsync();
            });

        private async Task<IList<DressCode>?> GetCachedDressCodes()
            => await _cache.GetOrCreateAsync("EventDressCodes", async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(12);
                return await _context.DressCodes
                    .AsNoTracking()
                    .ToListAsync();
            });

        private async Task<IList<Category>?> GetCachedCategories()
            => await _cache.GetOrCreateAsync("EventCategories", async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(12);
                return await _context.Categories
                    .AsNoTracking()
                    .ToListAsync();
            });

        private async Task<IList<RSVPState>?> GetCachedRSVPStates()
            => await _cache.GetOrCreateAsync("RsvpStates", async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(12);
                return await _context.RSVPStates
                    .AsNoTracking()
                    .ToListAsync();
            });

        private async Task<IList<CalendarEvent>?> GetCachedEventsForRegion(DateTime startTime, DateTime endTime, int regionId)
            => startTime >= DateTime.UtcNow.AddMonths(-1) && endTime <= DateTime.UtcNow.AddYears(1)
                ? (await _cache.GetOrCreateAsync($"Events:{regionId}", async entry =>
                    {
                        entry.AbsoluteExpiration = DateTime.UtcNow.AddMinutes(5);
                        //Cache events from one month in the past until one year in the future
                        return await _context.Events
                        .Include(e => e.Region)
                        .Include(e => e.Categories)
                        .Include(e => e.DressCodes)
                        .AsNoTracking()
                        .Where(e => e.EndDate >= DateTime.UtcNow.AddMonths(-1) && e.StartDate <= DateTime.UtcNow.AddYears(1) && e.RegionId == regionId && e.DeletedTS == null).ToListAsync();
                    }) ?? []).Where(e => e.StartDate >= startTime && e.StartDate <= endTime).ToList()
                : await _context.Events
                    .Include(e => e.Region)
                    .Include(e => e.Categories)
                    .Include(e => e.DressCodes)
                    .AsNoTracking()
                    .Where(e => e.StartDate >= startTime && e.StartDate <= endTime && e.RegionId == regionId && e.DeletedTS == null).ToListAsync();

        private async Task<IList<EventRSVP>?> GetCachedRSVPs(int eventId)
            => await _cache.GetOrCreateAsync($"Events:RSVPs:{eventId}", async entry =>
                {
                    entry.AbsoluteExpiration = DateTime.UtcNow.AddSeconds(30);
                    
                    return await _context.RSVPs
                        .AsNoTracking()
                        .Include(r => r.User)
                        .Include(r => r.State)
                        .Where(r => r.EventId == eventId)
                        .ToListAsync();
                });

        private async Task<Guid> AddEditEvent(CalendarEvent calendarEvent)
        {
            var local = _context.Set<CalendarEvent>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(calendarEvent.Id));

            // check if local is not null 
            if (local != null)
            {
                // detach
                _context.Entry(local).State = EntityState.Detached;
            }


            if (calendarEvent.Id == 0)
            {
                _context.Add(calendarEvent);
            }
            else
            {
                _context.Attach(calendarEvent);
            }

            await _context.SaveChangesAsync();

            _cache.Remove($"Events:{calendarEvent.RegionId}");

            return calendarEvent.UniqueId;
        }

        private async Task<int?> AddEditRSVP(EventRSVP rsvp)
        {
            var local = _context.Set<EventRSVP>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(rsvp.Id));

            // check if local is not null 
            if (local != null)
            {
                // detach
                _context.Entry(local).State = EntityState.Detached;
            }

            if (rsvp.Id == 0)
            {
                _context.Add(rsvp);
            }
            else
            {
                rsvp.Event = null;
                rsvp.User = null;
                rsvp.State = null;
                rsvp.CreatedUser = null;
                rsvp.UpdatedUser = null;
                _context.Attach(rsvp);
                _context.Entry(rsvp).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            _cache.Remove($"Events:RSVPs:{rsvp.EventId}");

            return rsvp.RSVPStateId;
        }

        public async Task<IEnumerable<Region>?> GetRegions()
            => await GetCachedRegions();

        public async Task<IEnumerable<Region>?> GetRegions(string? stateCode = null, string? name = null)
            => (await GetCachedRegions())?.Where(r =>
            (stateCode == null || (r.StateCode?.Equals(stateCode, StringComparison.OrdinalIgnoreCase) ?? false))
            && (name == null || (r.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) ?? false)));

        public async Task<IEnumerable<DressCode>?> GetDressCodes()
            => await GetCachedDressCodes();

        public async Task<IEnumerable<Category>?> GetCategories()
            => await GetCachedCategories();

        public async Task<CalendarEvent?> GetEventById(int? id = null, Guid? guid = null)
            => await _context.Events
                .Include(e => e.Region)
                .FirstOrDefaultAsync(e => e.Id == id || e.UniqueId == guid);

        public async Task<IEnumerable<CalendarEvent>?> GetEventsForRegion(DateTime startTime, DateTime endTime, int regionId)
            => await GetCachedEventsForRegion(startTime, endTime, regionId);

        public async Task<IEnumerable<CalendarEvent>?> GetEventsForState(DateTime startTime, DateTime endTime, string stateCode)
            => (await GetRegions(stateCode: stateCode))?.Select(async r => await GetEventsForRegion(startTime, endTime, r.Id)).Select(e => e.Result)?.SelectMany(e => e);

        public async Task<Guid> UpsertEvent(CalendarEvent calendarEvent)
            => await AddEditEvent(calendarEvent);

        public async Task<IEnumerable<RSVPState>?> GetRSVPStates()
            => await GetCachedRSVPStates();

        public async Task<IEnumerable<EventRSVP>?> GetRSVPsForEvent(int eventId)
            => await GetCachedRSVPs(eventId);

        public async Task<IEnumerable<EventRSVP>?> GetGoingsForEvent(int eventId)
            => (await GetCachedRSVPs(eventId) ?? []).Where(r => r.State.Name.Equals("going", StringComparison.OrdinalIgnoreCase) && r.DeletedTS == null);
        public async Task<IEnumerable<EventRSVP>?> GetInterestedsForEvent(int eventId)
            => (await GetCachedRSVPs(eventId) ?? []).Where(r => r.State.Name.Equals("interested", StringComparison.OrdinalIgnoreCase) && r.DeletedTS == null);

        public async Task<int?> UpsertRSVP(EventRSVP rsvp)
            => await AddEditRSVP(rsvp);
    }
}
