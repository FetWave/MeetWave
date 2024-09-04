using MeetWave.Data;
using MeetWave.Data.DTOs.Events;
using Google.Apis.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Caching.Memory;
using Radzen;
using MeetWave.Helper;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices.Marshalling;

namespace MeetWave.Services
{
    public class EventsService
    {
        private readonly IMemoryCache _cache;
        private readonly MeetWaveContext _context;

        public EventsService(IMemoryCache cache, MeetWaveContext context)
        {
            _cache = cache;
            _context = context;
        }

        private async Task<IList<Region>?> GetCachedRegions()
            => await _cache.GetOrCreateAsync("EventRegions", async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(12);
                return await _context.Regions
                    .ToListAsync();
            });

        private async Task<IList<DressCode>?> GetCachedDressCodes()
            => await _cache.GetOrCreateAsync("EventDressCodes", async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(12);
                return await _context.DressCodes
                    .ToListAsync();
            });

        private async Task<IList<Category>?> GetCachedCategories()
            => await _cache.GetOrCreateAsync("EventCategories", async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(12);
                return await _context.Categories
                    .ToListAsync();
            });

        private async Task<IList<RSVPState>?> GetCachedRSVPStates()
            => await _cache.GetOrCreateAsync("RsvpStates", async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(12);
                return await _context.RSVPStates
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
                        .Include(e => e.CreatedUser)
                        .Where(e => e.EndDate >= DateTime.UtcNow.AddMonths(-1) && e.StartDate <= DateTime.UtcNow.AddYears(1) && e.RegionId == regionId && e.DeletedTS == null).ToListAsync();
                    }) ?? []).Where(e => e.StartDate >= startTime && e.StartDate <= endTime).ToList()
                : await _context.Events
                    .Include(e => e.Region)
                    .Include(e => e.Categories)
                    .Include(e => e.DressCodes)
                    .Include(e => e.CreatedUser)
                    .Where(e => e.StartDate >= startTime && e.StartDate <= endTime && e.RegionId == regionId && e.DeletedTS == null).ToListAsync();

        private async Task<IList<EventRSVP>?> GetCachedRSVPs(int eventId)
            => await _cache.GetOrCreateAsync($"Events:RSVPs:{eventId}", async entry =>
                {
                    entry.AbsoluteExpiration = DateTime.UtcNow.AddSeconds(30);
                    
                    return await _context.RSVPs
                        .Include(r => r.User)
                        .Include(r => r.State)
                        .Include(r => r.CheckinCodes)
                        .Where(r => r.EventId == eventId)
                        .ToListAsync();
                });

        private async Task<Guid> AddEditEvent(CalendarEvent calendarEvent)
        {
            var localDressCodes = _context.Set<DressCode>()
                .Local
                .Where(d => calendarEvent.DressCodes?.Any(dc => dc.Id == d.Id) ?? false);

            // check if local is not null 
            if (localDressCodes?.Any() ?? false)
            {
                foreach (var entity in localDressCodes)
                {
                    _context.Entry(entity).State = EntityState.Unchanged;
                }
            }

            var localCategories = _context.Set<Category>()
                .Local
                .Where(c => calendarEvent.Categories?.Any(cg => cg.Id == c.Id) ?? false);

            // check if local is not null 
            if (localCategories?.Any() ?? false)
            {
                foreach (var entity in localCategories)
                {
                    _context.Entry(entity).State = EntityState.Unchanged;
                }
            }

            if (calendarEvent.Id == 0)
            {
                await _context.AddAsync(calendarEvent);
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
            var localEvent = _context.Set<CalendarEvent>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(rsvp.Event?.Id));

            // check if local is not null 
            if (localEvent != null)
            {
                // detach
                _context.Entry(localEvent).State = EntityState.Detached;
            }

            var localRegion = _context.Set<Region>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(rsvp.Event?.Region?.Id));

            // check if local is not null 
            if (localRegion != null)
            {
                // detach
                _context.Entry(localRegion).State = EntityState.Detached;
            }

            if (rsvp.Id == 0)
            {
                await _context.AddAsync(rsvp);
            }
            else
            {
                _context.Attach(rsvp);
                _context.Entry(rsvp).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            _cache.Remove($"Events:RSVPs:{rsvp.EventId}");

            return rsvp.RSVPStateId;
        }

        public async Task<string?> CreateCheckinCode(int rsvpId)
        {

            var rsvp = await _context.RSVPs.FirstOrDefaultAsync(r => r.Id == rsvpId);
            if (rsvp == null)
                return null;

            var code = RSVPHelper.GetCodeGenerator().Take(6).ToString();
            var c = await _context.AddAsync(new CheckinCode()
            {
                RsvpId = rsvpId,
                Code = code!
            });
            await _context.SaveChangesAsync();

            _cache.Remove($"Events:RSVPs:{rsvp.EventId}");

            return c.Entity.Code;
        }

        private async Task<IList<EventRSVP>> GetRsvpsForCheckinCode(int eventId, string code)
            => await _context.RSVPs
                .Include(r => r.CheckinCodes)
                .Include(r => r.User)
                .Where(r => r.Id == eventId && r.CheckinCodes != null && r.CheckinCodes.Any(c => EF.Functions.Like(c.Code, code)))
                .ToListAsync();

        public async Task<IList<EventRSVP>> GetRsvpsForCheckinCodeUnsafe(int eventId, string code)
        {
            var r = new Regex(@"^[a-zA-Z0-9]*$");
            return r.IsMatch(code) ? await GetRsvpsForCheckinCode(eventId, code) : throw new Exception("Invalid checkin code"); 
        }

        public async Task<bool> CheckIn(int id, Guid userId)
        {
            try
            {
                var checkin = await _context.CheckinCodes
                .Include(c => c.Rsvp)
                .FirstOrDefaultAsync(c => c.Id == id);

                if (checkin != null)
                {
                    var rsvp = checkin.Rsvp;
                    rsvp.CheckInTS = DateTime.UtcNow;
                    rsvp.CheckInUserId = userId.ToString();
                    await AddEditRSVP(rsvp);

                    return true;
                }
            }
            catch { }
            return false;
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
                .Include(e => e.CreatedUser)
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

        public async Task<IEnumerable<CalendarEvent>> GetOrganizingEvents(string userId, DateTime startDate, DateTime endDate)
            => await _context.Events
                .Include(e => e.Region)
                .Include(e => e.Categories)
                .Include(e => e.DressCodes)
                .Include(e => e.CreatedUser)
                .Where(e => e.CreatedUserId == userId && e.StartDate >= startDate && e.StartDate <= endDate)
                .ToListAsync();

        public async Task<IEnumerable<CalendarEvent>> GetRsvpedEvents(string userId, DateTime startDate, DateTime endDate)
           => await _context.Events
               .Include(e => e.RSVPs)
               .Include(e => e.Region)
               .Include(e => e.Categories)
               .Include(e => e.DressCodes)
               .Include(e => e.CreatedUser)
               .Where(e => e.RSVPs.Any(r => r.UserId == userId) && e.StartDate >= startDate && e.StartDate <= endDate)
               .ToListAsync();

    }
}
