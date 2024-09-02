using MeetWave.Data.DTOs.Profile;
using MeetWave.Data;
using Microsoft.Extensions.Caching.Memory;

namespace MeetWave.Services
{
    public class PaymentsService
    {
        private readonly MeetWaveContext _context;

        public PaymentsService(MeetWaveContext context)
        {
            _context = context;
        }
    }
}
