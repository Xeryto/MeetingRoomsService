using BusinessLogic.Models;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DAL
{
    public class FakeReservationRepository : FakeRepository<Reservation>, IReservationRepository
    {
        public FakeReservationRepository() { }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<List<Reservation>> GetInInterval(int roomId, DateTime from, DateTime to)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return Query().Where(x => x.MeetingRoomId == roomId && x.TimeFrom < to && x.TimeTo > from)
                .ToList();
        }
    }
}
