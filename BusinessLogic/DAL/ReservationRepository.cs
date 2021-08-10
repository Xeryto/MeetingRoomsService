using BusinessLogic.Models;
using DataAccessLayer;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DAL
{
    public class ReservationRepository : GenericRepository<Reservation>, IReservationRepository
    { 
        public ReservationRepository(MeetingRoomContext context) : base(context)
        {
            
        }

        public override async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            try
            {
                return await Query().Include(x => x.MeetingRoom).Include(x => x.User).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve entities: {ex.Message}");
            }
        }
        public override async Task<Reservation> GetByIdAsync(int id)
        {
            var reservation = await Query().Include(x => x.MeetingRoom).Include(x => x.User)
                .Where(x => x.Id == id).FirstOrDefaultAsync();

            if (reservation == null) throw new Exception($"Entity with id {id} doesn't exist");

            return reservation;
        }

        public async Task<List<Reservation>> GetInInterval(int roomId, DateTime from, DateTime to)
        {
            return await Query().Where(x => x.MeetingRoomId == roomId && x.TimeFrom < to && x.TimeTo > from)
                .ToListAsync();
        }
    }
}
