using MeetWave.Data;
using MeetWave.Data.DTOs.Payments;
using Microsoft.EntityFrameworkCore;
using MeetWave.Services.Interfaces;

namespace MeetWave.Services
{
    public class OrdersService
    {
        private readonly MeetWaveContext _context;
        private readonly IPaymentsService _payments;
        private string returnUrl;

        public OrdersService(MeetWaveContext context, IPaymentsService payments, IConfiguration configuration)
        {
            _context = context;
            _payments = payments;
            returnUrl = configuration["PaymentReturnUrl"] ?? throw new Exception("Must specify payment return URL");
        }

        public async Task<OrderReceipt?> GetReceipt(string receiptId)
            => await _context.Receipts
                .Include(r => r.Order)
                .Include(r => r.Order.LineItems)
                .Include(r => r.Order.Event)
                .FirstOrDefaultAsync(r => r.ReceiptId == receiptId);

        public async Task<OrderReceipt?> GetReceiptByOrderId(int orderId)
            => await _context.Receipts
                .Include(r => r.Order)
                .Include(r => r.Order.LineItems)
                .Include(r => r.Order.Event)
                .FirstOrDefaultAsync(r => r.OrderId == orderId);

        public async Task<Order?> GetOrderById(int id)
            => await _context.Orders
                .Include(o => o.LineItems)
                .Include(o => o.Event)
                .Include(o => o.Receipt)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<IList<Order>?> GetOrdersByUser(Guid userId)
            => await _context.Orders
                .Include(o => o.LineItems)
                .Include(o => o.Event)
                .Include(o => o.Receipt)
                .Where(o => o.UserId == userId.ToString())
                .ToListAsync();

        public async Task<IList<Order>?> GetOrdersByUserAndEventId(Guid userId, int eventId)
            => await _context.Orders
                .Include(o => o.LineItems)
                .Include(o => o.Event)
                .Include(o => o.Receipt)
                .Where(o => o.UserId == userId.ToString() && o.EventId == eventId)
                .ToListAsync();

        public async Task<IList<Order>?> GetOrdersByEventId(int eventId)
            => await _context.Orders
                .Include(o => o.LineItems)
                .Include(o => o.Event)
                .Include(o => o.Receipt)
                .Where(o => o.EventId == eventId)
                .ToListAsync();

        public async Task<Order?> CreateOrder(int eventId, IEnumerable<Helper.PaymentWrapper.LineItem> lineItems, Guid userId, Guid? createdUserId = null)
        {

            if (eventId <= 0 || userId == default || !(lineItems?.Any() ?? false))
                throw new ArgumentNullException();

            var session = await _payments.ChargeEventCover(lineItems, null, 0, returnUrl);
            if (session == null)
                return null;

            var createdUserIdString = (createdUserId ?? userId).ToString();

            var order = await _context.AddAsync(new Order()
            {
                CreatedUserId = createdUserIdString,
                EventId = eventId,
                PaymentUrl = session.ChargeUrl,
                UserId = userId.ToString()
            });

            await _context.SaveChangesAsync();

            var li = lineItems.Select(i => new OrderLineItem()
            {
                CreatedUserId = createdUserIdString,
                ItemName = i.Name,
                ItemPriceCents = i.UnitPriceCents,
                ItemQuantity = i.Quantity,
                OrderId = order.Entity.Id
            });
            await _context.AddRangeAsync(li);

            var receipt = await _context.AddAsync(new OrderReceipt()
            {
                CreatedUserId = createdUserIdString,
                OrderId = order.Entity.Id,
                ReceiptId = session.ReceiptId
            });
            await _context.SaveChangesAsync();

            return await GetOrderById(order.Entity.Id);
        }

        public async Task<Order?> MarkReceiptAsPaid(int orderId)
        {
            var receipt = await _context.Receipts.FirstOrDefaultAsync(r => r.OrderId == orderId);
            if (receipt == null)
                throw new InvalidDataException($"No order with order id {orderId}");
            receipt.PaidTS = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetOrderById(orderId);
        }
    }
}
