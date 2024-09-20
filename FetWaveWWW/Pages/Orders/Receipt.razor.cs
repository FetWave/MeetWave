using MeetWave.Data.DTOs.Payments;
using MeetWave.Helper;
using MeetWave.Services;
using MeetWave.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using System.Web;

namespace MeetWave.Pages.Orders
{
    public partial class Receipt : ComponentBase
    {
        [Parameter]
        public int? orderId { get; set; }
        [Parameter]
        public string? receiptId { get; set; }
        [SupplyParameterFromQuery]
        [Parameter]
        public string? session_id { get; set; }
        [Parameter]
        public string? calendarEventId { get; set; }

#nullable disable
        [Inject]
        public AuthHelperService Auth { get; set; }
        [Inject]
        private NavigationManager Navigation { get; set; }
        [Inject]
        private OrdersService Orders { get; set; }
        [Inject]
        private IPaymentsService Payments { get; set; }
#nullable enable

        protected override async Task OnInitializedAsync()
        {
            if (Guid.TryParse(await Auth.GetUserId(), out var userId))
                UserId = userId;

            if (!string.IsNullOrWhiteSpace(session_id) && string.IsNullOrWhiteSpace(receiptId))
                receiptId = session_id;

            if (receiptId != null)
                orderReceipts = [await Orders.GetReceipt(receiptId)];
            else if (orderId != null)
                orderReceipts = [await Orders.GetReceiptByOrderId(orderId.Value)];
            else if (calendarEventId != null && int.TryParse(calendarEventId, out var eventId))
                orderReceipts = (await Orders.GetOrdersByUserAndEventId(UserId.Value, eventId))?.Select(o => o.Receipt);

            if (UserId == null || orderReceipts == null || orderReceipts.First()!.Order.UserId != UserId.ToString())
            {
                Navigation.NavigateTo("/");
                return;
            }

            foreach(var receipt in orderReceipts.Where(r => r != null))
            {
                var details = await Payments.GetReceiptDetails(receipt!.ReceiptId!);
                if (receipt?.PaidTS == null && details?.Status == ReceiptDetails.PaidStatus.Paid)
                {
                    await Orders.MarkReceiptAsPaid(receipt!.OrderId);
                }
            }
        }

        private Guid? UserId { get; set; }
        private IEnumerable<OrderReceipt?>? orderReceipts { get; set; }

        private string FormatOrder(Order order)
        {
            return $"<p>Invoice Sent On {order.Receipt.CreatedTS}</p><ul>" + string.Join("<br/>", (order.LineItems ?? []).Select(li => $"<li>{HttpUtility.HtmlEncode(li.GetName())} x {li.ItemQuantity} @ {StringHelper.GetDisplayPriceFromCents(li.GetPriceCents())}</li>")) + "</ul>"
                + $"<p>Total : {StringHelper.GetDisplayPriceFromCents(order.LineItems.Sum(li => li.GetPriceCents() * li.ItemQuantity))}</p>";
        }
    }
}
