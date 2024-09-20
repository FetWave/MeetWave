using MeetWave.Data;
using MeetWave.Data.DTOs.Payments;
using Microsoft.EntityFrameworkCore;
using MeetWave.Services.Interfaces;
using System.Collections.Generic;
using MeetWave.Data.DTOs.Events;

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
                .ThenInclude(li => li.Inventory)
                .Include(r => r.Order.Event)
                .FirstOrDefaultAsync(r => r.ReceiptId == receiptId);

        public async Task<OrderReceipt?> GetReceiptByOrderId(int orderId)
            => await _context.Receipts
                .Include(r => r.Order)
                .Include(r => r.Order.LineItems)
                .ThenInclude(li => li.Inventory)
                .Include(r => r.Order.Event)
                .FirstOrDefaultAsync(r => r.OrderId == orderId);

        public async Task<Order?> GetOrderById(int id)
            => await _context.Orders
                .Include(o => o.LineItems)
                .ThenInclude(li => li.Inventory)
                .Include(o => o.Event)
                .Include(o => o.Receipt)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<IList<Order>?> GetOrdersByUser(Guid userId)
            => await _context.Orders
                .Include(o => o.LineItems)
                .ThenInclude(li => li.Inventory)
                .Include(o => o.Event)
                .Include(o => o.Receipt)
                .Where(o => o.UserId == userId.ToString())
                .ToListAsync();

        public async Task<IList<Order>?> GetOrdersByUserAndEventId(Guid userId, int eventId)
            => await _context.Orders
                .Include(o => o.LineItems)
                .ThenInclude(li => li.Inventory)
                .Include(o => o.Event)
                .Include(o => o.Receipt)
                .Where(o => o.UserId == userId.ToString() && o.EventId == eventId)
                .ToListAsync();

        public async Task<long> GetOrderedCountForInventory(int inventoryId)
            => await _context.Inventory
                .Include(o => o.LineItems)
                .Where(i => i.Id == inventoryId)
                .SelectMany(i => i.LineItems ?? new OrderLineItem[] { new() { ItemQuantity = 0} })
                .SumAsync(li => li.ItemQuantity);

        public async Task<bool> InventoryAvailable(int inventoryId)
            => await GetOrderedCountForInventory(inventoryId) < ((await _context.Inventory.FirstOrDefaultAsync(i => i.Id == inventoryId))?.ItemAvailableCount ?? int.MaxValue);

        public async Task<IList<Order>?> GetOrdersByEventId(int eventId)
            => await _context.Orders
                .Include(o => o.LineItems)
                .ThenInclude(li => li.Inventory)
                .Include(o => o.Event)
                .Include(o => o.Receipt)
                .Where(o => o.EventId == eventId)
                .ToListAsync();
        
        private async Task<bool> IsInventoryAvailable(IEnumerable<Helper.PaymentWrapper.InventoryLineItem> lineItems)
        {
            foreach (var li in lineItems)
            {
                var inventory = await _context.Inventory
                    .Include(i => i.LineItems)
                    .FirstOrDefaultAsync(i => i.Id == li.InventoryId);
                if (inventory?.ItemAvailableCount > 0 && inventory.LineItems?.Count > 0 && !await InventoryAvailable(li.InventoryId))
                {
                    return false;
                }
            }
            return true;
        }


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

        public async Task<Order?> CreateOrder(int eventId, IEnumerable<Helper.PaymentWrapper.InventoryLineItem> invLineItems, Guid userId)
        {

            if (eventId <= 0 || userId == default || !(invLineItems?.Any() ?? false))
                throw new ArgumentNullException();

            if (!await IsInventoryAvailable(invLineItems))
            {
                return null;
            }
            var lineItems = invLineItems.Select(li => new Helper.PaymentWrapper.LineItem(li));

            var session = await _payments.ChargeEventCover(lineItems, null, 0, returnUrl);
            if (session == null)
                return null;

            var createdUserIdString = userId.ToString();

            var order = await _context.AddAsync(new Order()
            {
                CreatedUserId = createdUserIdString,
                EventId = eventId,
                PaymentUrl = session.ChargeUrl,
                UserId = userId.ToString()
            });

            await _context.SaveChangesAsync();

            var li = invLineItems.Select(i => new OrderLineItem()
            {
                CreatedUserId = createdUserIdString,
                InventoryId = i.InventoryId,
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

        public async Task<IList<EventInventory>?> GetInventoryForEvent(int eventId)
            => await _context.Inventory.Where(i => i.EventId == eventId && i.DeleteTS == null).OrderBy(i => i.Priority ?? int.MaxValue).ToListAsync();

        public async Task<EventInventory> UpsertInventory(EventInventory inventory)
        {
            var localEvent = _context.Set<CalendarEvent>()
                .Local
                .FirstOrDefault(d => d.Id == inventory.EventId);

            // check if local is not null 
            if (localEvent != null)
            {
                _context.Entry(localEvent).State = EntityState.Unchanged;
            }

            if (inventory.Id == 0)
            {
                await _context.AddAsync(inventory);
            }
            else
            {
                _context.Attach(inventory);
                _context.Entry(inventory).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            return inventory;
        }

        public async Task<bool> DeleteInventory(int inventoryId, Guid userId)
        {
            var i = await _context.Inventory.FirstOrDefaultAsync(i => i.Id == inventoryId);
            if (i == null || i.DeleteTS != null)
                return false;
            i.DeleteTS = DateTime.Now;
            i.DeleteUserId = userId.ToString(); 
                
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
