using BusinessLogic.DAL;
using BusinessLogic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ReservationService
    {
        private readonly IGenericRepository<Reservation> _genericRepository;
        private readonly TimeSpan MaximumReservationTime = new TimeSpan(3, 0, 0);

        public ReservationService (IGenericRepository<Reservation> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<ActionResult<IEnumerable<Reservation>>> GetAll()
        {
            return await _genericRepository.Query().Include(x => x.MeetingRoom)
                .Include(x => x.User).ToListAsync();
        }

        public async Task<Reservation> GetById(int id)
        {
            return await _genericRepository.Query().Include(x => x.MeetingRoom)
                .Include(x => x.User).Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Reservation>> GetInInterval(MeetingRoom room, DateTime to, DateTime from)
        {
            return await _genericRepository.Query().Where(x => x.MeetingRoomId == room.Id && x.TimeFrom < to && x.TimeTo > from).ToListAsync();
        }

        public async Task<bool> checkChoosedData(ReservationUpdateModel reserve)
        {
            var query = _genericRepository.Query()
                    .Where(x => x.MeetingRoomId == reserve.MeetingRoomId && x.TimeFrom < reserve.To && x.TimeTo > reserve.From);
            if (reserve.Id != 0)
                query = query.Where(x => x.Id != reserve.Id);
            Console.WriteLine($"{reserve.From < DateTime.Now}, {reserve.To < reserve.From}, {reserve.To.Subtract(reserve.From) > MaximumReservationTime}, {await query.AnyAsync()}");
            return reserve.From < DateTime.Now || reserve.To < reserve.From || reserve.To.Subtract(reserve.From) > MaximumReservationTime || await query.AnyAsync();
        }

        public async Task<Reservation> Add(Reservation reservation)
        {
            await _genericRepository.AddAsync(reservation);
            return reservation;
        }

        public async Task<Reservation> Update(Reservation reservation)
        {
            return await _genericRepository.UpdateAsync(reservation);
        }

        public async Task<Reservation> Delete(int id)
        {
            return await _genericRepository.Delete(id);
        }
    }
}
