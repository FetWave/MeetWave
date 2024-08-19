using FetWaveWWW.Data;
using FetWaveWWW.Data.DTOs.Messages;
using FetWaveWWW.Helper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FetWaveWWW.Services
{
    public class MessagesService
    {
        private readonly FetWaveWWWContext _context;

        public MessagesService(FetWaveWWWContext context)
        {
            _context = context;
        }

        private async Task<MessageThread?> GetThread(string userId, long threadId)
            => await _context.MessageThreads
                .Include(t => t.CreatedUser)
                .Include(t => t.Recipients)
                .Include(t => t.Lines)
                .ThenInclude(l => l.Author)
                .Include(t => t.Lines)
                .ThenInclude(l => l.Reads)
                .FirstOrDefaultAsync(t => t.Id == threadId && (t.CreatedUserId == userId || t.Recipients.Any(r => r.RecipientUserId == userId && r.RemovedTS == null)));
        private async Task<IEnumerable<long>> GetRecentThreadIds(string userId, int takeCount = 100)
            => await _context.MessageThreads
                .Include(t => t.CreatedUser)
                .Include(t => t.Recipients)
                .Include(t => t.Lines)
                .ThenInclude(l => l.Author)
                .Where(t => t.CreatedUserId == userId || t.Recipients.Any(r => r.RecipientUserId == userId && r.RemovedTS == null))
                .OrderByDescending(t => t.Lines.Max(l => l.CreatedTS))
                .Select(t => t.Id)
                .Take(takeCount)
                .ToListAsync();

        private async Task<IEnumerable<MessageLine>> GetRecentMessageLines(string userId, long threadId, int takeCount = 100)
            => await _context.MessageThreads
                .Include(t => t.CreatedUser)
                .Include(t => t.Recipients)
                .Include(t => t.Lines)
                .ThenInclude(l => l.Reads)
                .Include(t => t.Lines)
                .ThenInclude(l => l.Author)
                .Where(t => t.Id == threadId && (t.CreatedUserId == userId || t.Recipients.Any(r => r.RecipientUserId == userId && r.RemovedTS == null)))
                .SelectMany(t => t.Lines)
                .OrderByDescending(l => l.CreatedTS)
                .Take(takeCount)
                .ToListAsync();

        public async Task<MessageWrapper?> GetMessageThread(string userId, long threadId, int takeCount = 100)
        {
            var thread = await GetThread(userId, threadId);
            if (thread == null)
                return null;

            var lines = thread.Lines.OrderByDescending(l => l.CreatedTS).Take(takeCount);
            return new()
            {
                Thread = thread,
                Lines = lines,
                LastMessageTS = lines?.Max(l => l.CreatedTS),
            };
        }

        public async Task<IEnumerable<MessageWrapper?>?> GetMessages(string userId, int threadTakeCount = 100, int lineTakeCount = 100)
        {
            var threads = await GetRecentThreadIds(userId, threadTakeCount);
            var messages = new List<MessageWrapper?>();
            foreach (var  threadId in threads)
            {
                messages.Add(await GetMessageThread(userId, threadId, lineTakeCount));
            }
            return messages.OrderByDescending(t => t?.LastMessageTS);
        }

        public async Task<bool> SendMessage(string senderId, string recipientId, string? subject, string body, long? threadId = null)
        {
            try
            {
                MessageThread? thread;
                if (threadId != null)
                {
                    thread = await GetThread(senderId, threadId.Value);
                }
                else
                {
                    var threadEntity = await _context.AddAsync<MessageThread>(new()
                    {
                        CreatedUserId = senderId,
                        Subject = subject
                    });
                    await _context.SaveChangesAsync();
                    thread = threadEntity.Entity;

                    var recipient = await _context.AddAsync<MessageRecipient>(new()
                        {
                            ThreadId = thread.Id,
                            AddedByUserId = senderId,
                            RecipientUserId = recipientId
                        });
                        await _context.SaveChangesAsync();
                }

                var line = await _context.AddAsync<MessageLine>(new()
                {
                    ThreadId = thread?.Id ?? throw new Exception("No MessageThread Specified"),
                    CreatedUserId = senderId,
                    LineText = body
                });
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public async Task<bool> StartGroupMessage(string senderId, IEnumerable<string> recipientIds, string? subject, string body)
        {
            try
            {
                var thread = await _context.AddAsync<MessageThread>(new()
                {
                    CreatedUserId = senderId,
                    Subject = subject
                });
                await _context.SaveChangesAsync();

                var recipients = recipientIds.Select(r => new MessageRecipient()
                {
                    ThreadId = thread.Entity.Id,
                    AddedByUserId = senderId,
                    RecipientUserId = r
                });
                await _context.AddRangeAsync(recipients);
                await _context.SaveChangesAsync();

                var line = await _context.AddAsync<MessageLine>(new()
                {
                    ThreadId = thread.Entity.Id,
                    CreatedUserId = senderId,
                    LineText = body
                });
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public async Task<bool> StartGroupMessageBCC(string senderId, IEnumerable<string> recipientIds, string? subject, string body)
        {
            try
            {
                var recipientsArray = recipientIds.ToArray();
                var threads = recipientIds.Select(r => new MessageThread()
                {
                    CreatedUserId = senderId,
                    Subject = subject
                }).ToArray();
                await _context.AddRangeAsync(threads);
                await _context.SaveChangesAsync();

                var recipients = Enumerable.Range(0, threads.Count()).Select(r => new MessageRecipient()
                {
                    ThreadId = threads[r].Id,
                    AddedByUserId = senderId,
                    RecipientUserId = recipientsArray[r]
                });
                await _context.AddRangeAsync(recipients);
                await _context.SaveChangesAsync();

                var lines = threads.Select(t => new MessageLine()
                {
                    ThreadId = t.Id,
                    CreatedUserId = senderId,
                    LineText = body
                });
                await _context.AddRangeAsync(lines);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }
    }
}
