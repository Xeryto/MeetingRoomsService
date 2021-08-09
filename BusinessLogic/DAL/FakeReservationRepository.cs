using BusinessLogic.Models;
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

        public async Task<List<Reservation>> GetInInterval(int roomId, DateTime from, DateTime to)
        {
            return Query().Where(x => x.MeetingRoomId == roomId && x.TimeFrom < to && x.TimeTo > from)
                .ToList();
        }
    }
}
