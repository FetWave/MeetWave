using MeetWave.Data.DTOs.Events;
using MeetWave.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace MeetWave.Pages.Admin
{
    public partial class Users : ComponentBase
    {
        [Inject]
        public UserManager<IdentityUser> _userManager { get; set; }
        [Inject]
        public AuthHelperService _auth { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (await _auth.HasRole("Administrator"))
            {
                await RefreshList();
            }
        }

        private async Task RefreshList()
        {
            UserList = _userManager.Users.ToArray();
            Roles = [];
            foreach (var user in UserList)
            {
                Roles.Add(user.Id, await GetRoles(user));
            }
            StateHasChanged();
        }

        private IEnumerable<IdentityUser> UserList = [];
        private Dictionary<string, string> Roles = [];

        public async Task<string> GetRoles(IdentityUser user)
            => string.Join(", ", await _userManager.GetRolesAsync(user));

        public async Task Delete(string userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is not null)
            {
                var rolesForUser = await _userManager.GetRolesAsync(user);
                if (rolesForUser?.Any() ?? false)
                {
                    foreach (var item in rolesForUser.ToList())
                    {
                        await _userManager.RemoveFromRoleAsync(user, item);
                    }
                }

                await _userManager.DeleteAsync(user);
            }
            await RefreshList();
        }

        public async Task RoleChange(string userId, string role)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is not null)
            {
                var rolesForUser = await _userManager.GetRolesAsync(user);
                if (rolesForUser?.Any(r => r.Equals(role, StringComparison.OrdinalIgnoreCase)) ?? false)
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }
            await RefreshList();
        }

        public async Task ToggleAdmin(string userId)
            => await RoleChange(userId, "Administrator");
        public async Task ToggleModerator(string userId)
           => await RoleChange(userId, "Moderator");
    }
}
