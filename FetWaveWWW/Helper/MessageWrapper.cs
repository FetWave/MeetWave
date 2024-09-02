using MeetWave.Data.DTOs.Messages;

namespace MeetWave.Helper
{
    public class MessageWrapper
    {
        public MessageThread? Thread { get; set; }
        public DateTime? LastMessageTS { get; set; }
        public IEnumerable<MessageLine?>? Lines { get; set; }
    }
}
