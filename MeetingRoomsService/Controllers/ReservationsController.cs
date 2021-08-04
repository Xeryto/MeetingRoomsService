using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeetingRoomsService.DAL;
using MeetingRoomsService.Models;

namespace MeetingRoomsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IGenericRepository<Reservation> _genericRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<MeetingRoom> _meetingRoomRepository;
        private readonly TimeSpan MaximumReservationTime = new TimeSpan(3,0,0);

        public ReservationsController(IGenericRepository<Reservation> genericRepository,
            IGenericRepository<User> userRepository, IGenericRepository<MeetingRoom> meetingRoomRepository)
        {
            _genericRepository = genericRepository;
            _userRepository = userRepository;
            _meetingRoomRepository = meetingRoomRepository;
        }

        // GET: api/Reservations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            return await _genericRepository.Query().Include(x => x.MeetingRoom).
                Include(x => x.User).ToListAsync();
        }

        // GET: api/Reservations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            return await _genericRepository.Query().Include(x => x.MeetingRoom).
                Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
        }


        // POST: api/Reservations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostReservation(ReservationPostModel reserve)
        {
            var user = await _userRepository.GetByIdAsync(reserve.UserId);
            var room = await _meetingRoomRepository.GetByIdAsync(reserve.MeetingRoomId);
            if (reserve.From < DateTime.Now || reserve.To < reserve.From || 
                reserve.From.Subtract(reserve.To) > MaximumReservationTime) return Ok("Choose appropriate time and date");
            var reservation = new Reservation
            {
                User = user,
                MeetingRoom = room,
                TimeFrom = reserve.From,
                TimeTo = reserve.To
            };
            var res = await _genericRepository.Query().Where(x => x.MeetingRoomId == reserve.MeetingRoomId).ToListAsync();
            foreach (var r in res)
            {
                if ((reservation.TimeFrom <= r.TimeFrom && r.TimeFrom < r.TimeTo) ||
                    (reservation.TimeFrom < r.TimeTo && r.TimeTo < reservation.TimeTo)) return Ok("Time is taken");
            }

            await _genericRepository.AddAsync(reservation);

            return Ok(reservation.Id);
        }

        // DELETE: api/Reservations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            await _genericRepository.Delete(id);

            return NoContent();
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateAsync(ReservationUpdateModel reserve)
        {
            var user = await _userRepository.GetByIdAsync(reserve.UserId);
            var room = await _meetingRoomRepository.GetByIdAsync(reserve.MeetingRoomId);
            if (reserve.From < DateTime.Now || reserve.To < reserve.From ||
               reserve.From.Subtract(reserve.To) > MaximumReservationTime) return Ok("Choose appropriate time and date");
            var reservation = new Reservation
            {
                Id = reserve.Id,
                User = user,
                MeetingRoom = room,
                TimeFrom = reserve.From,
                TimeTo = reserve.To
            };
            var res = await _genericRepository.Query().Where(x => x.MeetingRoomId == reserve.MeetingRoomId).ToListAsync();
            foreach (var r in res)
            {
                if ((reservation.TimeFrom <= r.TimeFrom && r.TimeFrom < r.TimeTo) ||
                    (reservation.TimeFrom < r.TimeTo && r.TimeTo < reservation.TimeTo)) return Ok("Time is taken");
            }

            await _genericRepository.UpdateAsync(reservation);
            return Ok(reservation.Id);
        }
       
    }
}
