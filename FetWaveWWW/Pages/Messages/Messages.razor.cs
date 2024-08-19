using FetWaveWWW.Data.DTOs.Messages;
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
        RadzenDataList<MessageLine> dataList;
        IEnumerable<MessageLine> lines;
        private string newMessage = string.Empty;

        private async Task RefreshCurrentChat(long threadId)
        {
            UserMessages = await MessagesService.GetMessages(UserId.ToString()!); 
            lines = (UserMessages?.FirstOrDefault(m => m?.Thread?.Id == threadId)?.Lines ?? [])?
                .Where(l => !string.IsNullOrEmpty(l?.LineText)).Select(l => l!) ?? [];
        }

        private async Task OnMessageOpen(long threadId)
        {

            lines = (UserMessages?.FirstOrDefault(m => m?.Thread?.Id == threadId)?.Lines ?? [])?
                .Where(l => !string.IsNullOrEmpty(l?.LineText)).Select(l => l!) ?? [];
        }

        private async Task SendMessage(long threadId)
        {
            await MessagesService.SendMessage(UserId.ToString()!, newMessage, threadId: threadId);
            await RefreshCurrentChat(threadId);
            StateHasChanged();

        }
    }
}
