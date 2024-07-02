using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace FetWaveWWW.Services
{
    public class AuthHelperService
    {
        private readonly AuthenticationStateProvider _provider;
        public AuthHelperService(AuthenticationStateProvider stateProvider)
        {
            _provider = stateProvider;
        }

        public async Task<string?> GetUserId()
             => (await _provider.GetAuthenticationStateAsync()).User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}
