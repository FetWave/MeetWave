using MeetWave.Data.DTOs.Events;
using MeetWave.Data.DTOs.Messages;
using MeetWave.Data.DTOs.Payments;
using MeetWave.Helper;
using MeetWave.Services;
using MeetWave.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Radzen.Blazor;
using System.Text;
using System.Web;
using static MeetWave.Helper.PaymentWrapper;

namespace MeetWave.Pages.Events
{
    [Authorize]
    public partial class EventView : ComponentBase
    {
#nullable disable
        [Inject]
        public EventsService Events { get; set; }
        [Inject]
        public IConfiguration Configuration { get; set; }
        [Inject]
        public AuthHelperService Auth { get; set; }
        [Inject]
        private NavigationManager Navigation { get; set; }
        [Inject]
        public IExternalEmailSender Emails { get; set; }
        [Inject]
        private OrdersService Orders { get; set; }
        [Inject]
        public MessagesService Messages { get; set; }
#nullable enable

        [Parameter]
        public string? eventGuid { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (Guid.TryParse(await Auth.GetUserId(), out var userId))
            {
                UserId = userId;
            }

            if (Guid.TryParse(eventGuid, out var eventId))
            {
                calendarEvent = await Events.GetEventById(guid: eventId);
            }

            if (calendarEvent == null)
            {
                Navigation.NavigateTo("/events");
            }

            if (userId.ToString().Equals(calendarEvent?.CreatedUserId, StringComparison.OrdinalIgnoreCase))
            {
                Organizer = true;
                EventOrders = await Orders.GetOrdersByEventId(calendarEvent.Id);
                RSVPs = await Events.GetRSVPsForEvent(calendarEvent.Id);
                SelectedRSVPs = RSVPs?.Select(r => r.Id).ToDictionary(x => x!, _ => false) ?? [];
            }

            GoingHTML = await HtmlHelper.GetRsvpMemberList(Events, calendarEvent!.Id, calendarEvent.CreatedUserId == UserId.ToString(), RsvpStateEnum.Going);
            InterestedHTML = await HtmlHelper.GetRsvpMemberList(Events, calendarEvent.Id, calendarEvent.CreatedUserId == UserId.ToString(), RsvpStateEnum.Interested);

            UserRsvp = (await Events.GetRSVPsForEvent(calendarEvent.Id) ?? []).FirstOrDefault(r => r.UserId == UserId.ToString());
            UserOrders = await Orders.GetOrdersByUserAndEventId(userId, calendarEvent.Id);
        }

        private bool Organizer { get; set; } = false;

        private CalendarEvent? calendarEvent { get; set; }

        private string? GoingHTML { get; set; }
        private string? InterestedHTML { get; set; }
        
        private Guid? UserId { get; set; }

        private EventRSVP? UserRsvp { get; set; }

        private IList<Order>? UserOrders { get; set; }
        private IList<Order>? EventOrders { get; set; }

        private IEnumerable<EventRSVP>? RSVPs { get; set; }
        private Dictionary<int, bool>? SelectedRSVPs { get; set; }
        private EmailListEnum? EmailList { get; set; } = EmailListEnum.All;
        private IEnumerable<IdentityUser>? InvoiceRecipients { get; set; }

        private string EmailSubject { get; set; } = string.Empty;
        private string EmailBody { get; set; } = string.Empty;

        private string? EmailFeedback { get; set; }

        private IList<LineItem>? LineItems { get; set; } = [];
        RadzenDataList<LineItem> dataList;

        private string OTP { get; set; } = string.Empty;
        private IList<EventRSVP> PendingCheckins { get; set; } = [];

        private async Task SubmitOTP()
        {
            if (!string.IsNullOrWhiteSpace(OTP))
            {
                var rsvp = await Events.GetRsvpsForCheckinCodeUnsafe(calendarEvent!.Id, OTP);
                if (rsvp?.Any() ?? false)
                {
                    PendingCheckins = rsvp;
                    StateHasChanged();
                }
            }
        }

        private async Task Checkin(int id)
        {
            PendingCheckins = [];
            await Events.CheckIn(id, UserId.Value);
            RSVPs = await Events.GetRSVPsForEvent(calendarEvent.Id);
            StateHasChanged();
        }

        private async Task UndoCheckin(int id)
        {
            await Events.UndoCheckin(id, UserId.Value);
            RSVPs = await Events.GetRSVPsForEvent(calendarEvent.Id);
            StateHasChanged();
        }

        private void OnNameChange(string name, LineItem li)
            => li.Name = name;

        private void DeleteLineItem(LineItem li)
            => LineItems.Remove(li);
        private void AddLineItem()
            => LineItems.Add(new());

        private void EmailListOnChange(ChangeEventArgs args)
        {
            EmailList = Enum.TryParse<EmailListEnum>(args.Value?.ToString(), out var value) ? value : null;
            UpdateRecipients();
        }

        private void UpdateRecipients()
        {
            InvoiceRecipients = EmailList switch
            {
                EmailListEnum.All => RSVPs?.Select(r => r.User).ToList(),
                EmailListEnum.Approved => RSVPs?.Where(r => r.ApprovedTS != null).Select(r => r.User).ToList(),
                EmailListEnum.Selected => RSVPs?.Where(r => (SelectedRSVPs?.TryGetValue(r.Id, out var selected) ?? false) ? selected : false).Select(r => r.User).ToList(),
                _ => null
            };
        }
        private async Task ApproveRSVP(int id)
        {
            var r = RSVPs?.FirstOrDefault(r => r.Id == id);
            if (r == null)
                return;
            r.ApprovedByUserId = UserId.ToString();
            r.ApprovedTS = DateTime.UtcNow;
            await Events.UpsertRSVP(r);
        }

        private async Task UnapproveRSVP(int id)
        {
            var r = RSVPs?.FirstOrDefault(r => r.Id == id);
            if (r == null)
                return;
            r.UpdatedUserId = UserId.ToString();
            r.ApprovedByUserId = null;
            r.ApprovedTS = null;
            await Events.UpsertRSVP(r);
        }

        private async Task UpdateRsvp(RsvpStateEnum? state, EventRSVP? rsvp, int? eventId)
        {
            if (state == null && rsvp != null)
            {
                await RSVPHelper.RemoveRSVP(Events, rsvp, UserId.ToString()!);
            }
            else if (state == RsvpStateEnum.Going)
            {
                await RSVPHelper.RSVPGoing(Events, rsvp, eventId, UserId.ToString());
            }
            else if (state == RsvpStateEnum.Interested)
            {
                await RSVPHelper.RSVPInterested(Events, rsvp, eventId, UserId.ToString());
            }
            else
            {
                //log error
            }
            UserRsvp = (await Events.GetRSVPsForEvent(calendarEvent!.Id) ?? []).FirstOrDefault(r => r.UserId == UserId.ToString());
            GoingHTML = await HtmlHelper.GetRsvpMemberList(Events, calendarEvent.Id, calendarEvent.CreatedUserId == UserId.ToString(), RsvpStateEnum.Going);
            InterestedHTML = await HtmlHelper.GetRsvpMemberList(Events, calendarEvent.Id, calendarEvent.CreatedUserId == UserId.ToString(), RsvpStateEnum.Interested);
            StateHasChanged();
        }

        private async Task SendEmail()
        {
            if (!ValidateLineItems())
            {
                EmailFeedback = "Must have line items to send invoice";
            }

            foreach (var user in InvoiceRecipients ?? [])
            {
                await Orders.CreateOrder(calendarEvent!.Id, LineItems!, Guid.Parse(user.Id), UserId);
            }

            var toEmails = InvoiceRecipients?.Select(u => u.Email).Where(e => !string.IsNullOrEmpty(e));
            var startTime = calendarEvent!.StartDate;
            var formattedBody = !string.IsNullOrEmpty(EmailBody)
                ? "<br/>"
                    + "~~~~~"
                    + "<br/>"
                    + "~~~~~"
                    + "<br/>"
                    + EmailBody
                : string.Empty;
            var contextBody = $"You are receiving this invoice from {calendarEvent!.CreatedUser!.UserName} for the event {calendarEvent.Title}."
                + "<br/>"
                + $"Happening on {startTime?.DayOfWeek} {startTime:MM/dd/yyyy} at {startTime:hh:mm tt}"
                + formattedBody
                + "<br/>"
                + "~~~~~"
                + "<br/>"
                + "~~~~~"
                + "<br/>"
                + $"<a href=\"{Navigation.BaseUri}invoices/{calendarEvent.Id}\">VIEW INVOICE</a>"
                + FormatLineItems();
            var contextSubject =   $"Fetwave event Invoice {(!string.IsNullOrEmpty(EmailSubject) ? (" - " + EmailSubject) : string.Empty)}";

            await Messages.StartGroupMessageBCC(UserId.ToString()!, InvoiceRecipients?.Select(r => r.Id) ?? [], contextSubject, contextBody);

            if (toEmails?.Any() ?? false)
            {
                await Emails.EmailListAsync(toEmails!, contextSubject, contextBody);
                EmailFeedback = "Email sent";
            }

        }

        private string FormatLineItems()
        {
            return "<ul>" + string.Join("<br/>", (LineItems ?? []).Select(li => $"<li>{HttpUtility.HtmlEncode(li.Name)} x {li.Quantity} @ ${(decimal)li.UnitPriceCents / 100:0.00}</li>")) + "</ul>" 
                + $"<p>Total : ${(decimal)(LineItems ?? []).Sum(li => li.GetTotal()) / 100:0.00}</p>";
        }

        private string FormatOrders(Order order)
        {
            return $"<p>Invoice Sent On {order.Receipt.CreatedTS}</p><ul>" + string.Join("<br/>", (order.LineItems ?? []).Select(li => $"<li>{HttpUtility.HtmlEncode(li.ItemName)} x {li.ItemQuantity} @ ${(decimal)li.ItemPriceCents / 100:0.00}</li>")) + "</ul>"
                + (order.Receipt.PaidTS == null ? $"<p>Unpaid  <a href=\"{order.PaymentUrl}\">PAY HERE</a>" : "PAID");
        }

        private bool ValidateLineItems()
        {
            if (!(LineItems?.Any() ?? false)) return false;

            foreach(var li in LineItems ?? [])
            {
                if (string.IsNullOrWhiteSpace(li.Name) || li.Quantity <= 0 || li.UnitPriceCents <= 0)
                    return false;
            }
            return true;
        }

        private async Task GenerateCode()
        {
            if (UserRsvp == null)
                return;

            await Events.CreateCheckinCode(UserRsvp.Id);
            StateHasChanged();
        }

        private string FormatRsvpListItem(EventRSVP r)
        {
            var orders = EventOrders?.Where(o => o.UserId == r.UserId);
            var sb = new StringBuilder();
            sb.Append("<ul>");
            sb.Append("<li>");
            sb.Append(r.ApprovedTS != null ? "Approved" : "Not Approved");
            sb.Append("</li>");
            sb.Append("<li>");
            if (orders?.Any(o => o.Receipt.PaidTS != null) ?? false)
            {
                sb.Append("Invoices paid");
                sb.Append("<ul>");
                foreach (var o in orders.Where(o => o.Receipt.PaidTS != null))
                {
                    sb.Append("<li>");
                    sb.Append(FormatInvoiceSummary(o));
                    sb.Append("</li>");
                }
                sb.Append("</ul>");
            }
            else
                sb.Append("No Invoices paid");
            sb.Append("</li>");
            sb.Append("</li>");
            return sb.ToString();
        }

        public string FormatInvoiceSummary(Order order)
            => $"Invoice Sent On {order.Receipt.CreatedTS} - " + string.Join(", ", order.LineItems.Select(li => $"{li.ItemName} x {li.ItemQuantity}"));


        private void GotoEditEvent()
        {
            Navigation.NavigateTo($"/event/manage/{eventGuid}");
        }
    }
}
