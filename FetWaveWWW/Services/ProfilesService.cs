using FetWaveWWW.Data;
using FetWaveWWW.Data.DTOs.Events;
using FetWaveWWW.Data.DTOs.Profile;
using Google.Apis.Gmail.v1.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
using System.Linq.Dynamic.Core;
using System.Runtime.CompilerServices;

namespace FetWaveWWW.Services
{
    public class ProfilesService
    {
        private readonly IMemoryCache _cache;
        private readonly FetWaveWWWContext _context;


        public static readonly UserProfile PrivateProfile
            = new()
            {
                User = new()
                {
                    UserName = "Private"
                },
                AboutMe = "Private Profile"
            };

        public ProfilesService(IMemoryCache cache, FetWaveWWWContext context)
        {
            _cache = cache;
            _context = context;
        }

        private async Task<IList<UserPronouns>?> GetCachedPronouns()
            => await _cache.GetOrCreateAsync("UserPronouns", async entry =>
            {
                entry.AbsoluteExpiration = DateTime.UtcNow.AddMinutes(5);
                return await _context.Pronouns
                    .Where(p => p.IsPublicTS != null)
                    .ToListAsync();
            });

        public async Task<UserProfile?> GetProfile(Guid userId)
            => await _context.Profiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId.ToString());


        public async Task<UserProfile?> GetProfile(string userName)
            => await _context.Profiles
                .Include(p => p.User)
                .Include(p => p.Region)
                .Include(p => p.Pronouns)
                .FirstOrDefaultAsync(p => p.User.UserName == userName);

        public async Task<IList<UserPronouns>?> GetPronounsList()
            => await GetCachedPronouns();

        public async Task<UserProfile> UpsertProfile(UserProfile profile)
        {
            if (profile.Id == 0)
                await _context.Profiles.AddAsync(profile);
            else
            {
                var localUser = _context.Set<IdentityUser>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(profile.User.Id));

                // check if local is not null 
                if (localUser != null)
                {
                    // detach
                    _context.Entry(localUser).State = EntityState.Detached;
                }
                _context.Attach(profile);

                _context.Entry(profile).State = EntityState.Modified;
            }
                
            await _context.SaveChangesAsync();
            return profile;
        }
    }
}
