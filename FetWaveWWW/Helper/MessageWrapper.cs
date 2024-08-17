using FetWaveWWW.Data.DTOs.Messages;

namespace FetWaveWWW.Helper
{
    public class MessageWrapper
    {
        public MessageThread? Thread { get; set; }
        public DateTime? LastMessageTS { get; set; }
        public IEnumerable<MessageLine?>? Lines { get; set; }
    }
}
