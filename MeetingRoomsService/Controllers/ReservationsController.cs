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

        public ReservationsController(IGenericRepository<Reservation> genericRepository, IGenericRepository<User> userRepository, IGenericRepository<MeetingRoom> meetingRoomRepository)
        {
            _genericRepository = genericRepository;
            _userRepository = userRepository;
            _meetingRoomRepository = meetingRoomRepository;
        }

        // GET: api/Reservations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            return await _genericRepository.Query().Include(x => x.MeetingRoom).Include(x => x.User).ToListAsync();
        }

        // GET: api/Reservations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            return await _genericRepository.Query().Include(x => x.MeetingRoom).Include(x => x.User).FirstOrDefaultAsync(x => x.ReservationId == id);
        }


        public class ReservationPostModel
        {
            public int UserId { get; set; }
            public int MeetingRoomId { get; set; }
            public DateTime From { get; set; }
            public DateTime To { get; set; }
        }
        // POST: api/Reservations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Reservation>> PostReservation(ReservationPostModel reserve)
        {
            var user = await _userRepository.GetByIdAsync(reserve.UserId);
            var room = await _meetingRoomRepository.GetByIdAsync(reserve.MeetingRoomId);
            var reservation = new Reservation
            {
                User = user,
                MeetingRoom = room,
                TimeFrom = reserve.From,
                TimeTo = reserve.To
            };

            await _genericRepository.AddAsync(reservation);

            return CreatedAtAction("GetReservation", new { id = reservation.ReservationId }, reservation);
        }

        // DELETE: api/Reservations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            await _genericRepository.Delete(id);

            return NoContent();
        }

        public class ReservationUpdateModel
        {
            public int ReservationId { get; set; }
            public int UserId { get; set; }
            public int MeetingRoomId { get; set; }
            public DateTime From { get; set; }
            public DateTime To { get; set; }
        }

        [HttpPatch]
        public async Task<ActionResult<Reservation>> UpdateAsync(ReservationUpdateModel reserve)
        {
            var user = await _userRepository.GetByIdAsync(reserve.UserId);
            var room = await _meetingRoomRepository.GetByIdAsync(reserve.MeetingRoomId);
            var reservation = new Reservation
            {
                ReservationId = reserve.ReservationId,
                User = user,
                MeetingRoom = room,
                TimeFrom = reserve.From,
                TimeTo = reserve.To
            };

            await _genericRepository.UpdateAsync(reservation);
            return CreatedAtAction("GetReservation", new { id = reservation.ReservationId }, reservation);
        }
       
    }
}
