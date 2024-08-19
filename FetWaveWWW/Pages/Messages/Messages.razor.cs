using FetWaveWWW.Helper;
using FetWaveWWW.Services;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen.Blazor.Rendering;

namespace FetWaveWWW.Pages.Messages
{
    public partial class Messages
    {
#nullable disable
        [Inject]
        public MessagesService MessagesService { get; set; }
        [Inject]
        public IConfiguration Configuration { get; set; }
        [Inject]
        public AuthHelperService Auth { get; set; }
        [Inject]
        private NavigationManager Navigation { get; set; }

        private Popup popup;
        private RadzenButton button;
#nullable enable

        protected override async Task OnInitializedAsync()
        {
            if (Guid.TryParse(await Auth.GetUserId(), out var userId))
            {
                UserId = userId;
            }
            else
            {
                Navigation.NavigateTo("/");
            }

            if (UserId != null)
            {
                UserMessages = await MessagesService.GetMessages(UserId.ToString()!);
            }
        }

        private Guid? UserId { get; set; }

        private IEnumerable<MessageWrapper?>? UserMessages { get; set; }
        private string newMessage = string.Empty;

        private async Task OnMessageOpen(long threadId)
        {

        }

        private async Task SendMessage(long threadId)
        {

        }
    }
}
