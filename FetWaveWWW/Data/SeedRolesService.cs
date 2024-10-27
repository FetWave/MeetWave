using MeetWave.Data.DTOs.Events;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace MeetWave.Data
{
    public class SeedRolesService
    {
        private class CustomRole
        {
            public string Name { get; set; }
            public IEnumerable<string> Users { get; set; }
        }

        private readonly UserManager<IdentityUser> userManeger;
        private readonly RoleManager<IdentityRole> roleManager;

        public SeedRolesService(UserManager<IdentityUser> _userManager, RoleManager<IdentityRole> _roleManager)
        {
            userManeger = _userManager;
            roleManager = _roleManager;
        }

        public async Task AddCustomRoles()
        {
            using var sr = new StreamReader("Data/SEED/Roles.json");
            var customRoles = JsonSerializer.Deserialize<IEnumerable<CustomRole>>(await sr.ReadToEndAsync());

            foreach (var r in customRoles ?? [])
            {
                var n = r.Name;
                if (string.IsNullOrWhiteSpace(n))
                    continue;

                var roleExists = await roleManager.RoleExistsAsync(n);
                if (!roleExists)
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(n));
                    if (!roleResult.Succeeded)
                        continue;
                }

                foreach(var u in r.Users)
                {
                    var user = await userManeger.FindByEmailAsync(u);
                    if (user != null)
                    {
                        await userManeger.AddToRoleAsync(user, n);
                    }
                }
            }
        }
    }
}
