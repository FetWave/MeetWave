using FetWaveWWW.Data.DTOs.Events;
using FetWaveWWW.Data.DTOs.Profile;
using Microsoft.Build.Evaluation;
using System.Security.AccessControl;
using System.Text.Json;

namespace FetWaveWWW.Data
{
    public class SeedDataService
    {
        private readonly FetWaveWWWContext _context;
        public SeedDataService(FetWaveWWWContext context)
        {
            _context = context;
        }

        public async Task SeedEventInfra()
        {
            await SeedCategories();
            await SeedDressCodes();
            await SeedRegions();
            await SeedRSVPStates();
        }

        public async Task SeedProfileInfra()
        {
            await SeedPronouns();
        }

        private async Task SeedCategories()
        {
            using var sr = new StreamReader("Data/SEED/Categories.json");
            var categories = JsonSerializer.Deserialize<IEnumerable<Category>>(await sr.ReadToEndAsync());

            var existingCats = _context.Categories.Select(c => c.Name).ToList();

            var newCats = categories?.Where(c => !existingCats.Any(e => e.Equals(c.Name, StringComparison.OrdinalIgnoreCase)));
            if (newCats?.Any() ?? false)
            {
                await _context.Categories.AddRangeAsync(newCats);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedDressCodes()
        {
            using var sr = new StreamReader("Data/SEED/DressCodes.json");
            var categories = JsonSerializer.Deserialize<IEnumerable<DressCode>>(await sr.ReadToEndAsync());

            var existingDCs = _context.DressCodes.Select(c => c.Name).ToList();

            var newDCs = categories?.Where(c => !existingDCs.Any(e => e.Equals(c.Name, StringComparison.OrdinalIgnoreCase)));
            if (newDCs?.Any() ?? false)
            {
                await _context.DressCodes.AddRangeAsync(newDCs);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedRegions()
        {
            using var sr = new StreamReader("Data/SEED/Regions.json");
            var categories = JsonSerializer.Deserialize<IEnumerable<Region>>(await sr.ReadToEndAsync());

            var existingRegions = _context.Regions.Select(c => new Tuple<string, string>(c.StateCode, c.Name)).ToList();

            var newRegions = categories?.Where(c => !existingRegions.Any(e =>
                e.Item1.Equals(c.StateCode, StringComparison.OrdinalIgnoreCase)
                && e.Item2.Equals(c.Name, StringComparison.OrdinalIgnoreCase)));

            if (newRegions?.Any() ?? false)
            {
                await _context.Regions.AddRangeAsync(newRegions);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedRSVPStates()
        {
            using var sr = new StreamReader("Data/SEED/RSVPStates.json");
            var states = JsonSerializer.Deserialize<IEnumerable<RSVPState>>(await sr.ReadToEndAsync());

            var existingstates = _context.RSVPStates.Select(c => c.Name).ToList();

            var newStates = states?.Where(c => !existingstates.Any(e => e.Equals(c.Name, StringComparison.OrdinalIgnoreCase)));
            if (newStates?.Any() ?? false)
            {
                await _context.RSVPStates.AddRangeAsync(newStates);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedPronouns()
        {
            using var sr = new StreamReader("Data/SEED/Pronouns.json");
            var pronouns = JsonSerializer.Deserialize<IEnumerable<UserPronouns>>(await sr.ReadToEndAsync());

            var existingPronouns = _context.Pronouns.Select(c => c.Value).ToList();

            var newPronouns = pronouns?.Where(c => !existingPronouns.Any(e => e.Equals(c.Value, StringComparison.OrdinalIgnoreCase)));
            if (newPronouns?.Any() ?? false)
            {
                foreach (var p in newPronouns)
                    p.IsPublicTS = DateTime.UtcNow;
                await _context.Pronouns.AddRangeAsync(newPronouns);
                await _context.SaveChangesAsync();
            }
        }
    }
}
