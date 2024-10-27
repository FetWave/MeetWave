using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace MeetWave.Services
{
    public class AuthHelperService
    {
        private readonly AuthenticationStateProvider _provider;
        public AuthHelperService(AuthenticationStateProvider stateProvider)
        {
            _provider = stateProvider;
        }

        public async Task<string?> GetUserClaim(string claimType)
            => (await _provider.GetAuthenticationStateAsync()).User.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;

        public async Task<string?> GetUserId()
             => await GetUserClaim(ClaimTypes.NameIdentifier);

        public async Task<string?> GetUserEmail()
            => await GetUserClaim(ClaimTypes.Email);

        public async Task<bool> HasRole(string role)
            => (await _provider.GetAuthenticationStateAsync()).User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == role);
    }
}
