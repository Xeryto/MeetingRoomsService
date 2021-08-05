using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeetingRoomsService.DAL;
using MeetingRoomsService.Models;
using Swashbuckle.AspNetCore.Annotations;
using AutoMapper;

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
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<Reservation>))]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            return await _genericRepository.Query().Include(x => x.MeetingRoom).
                Include(x => x.User).ToListAsync();
        }

        // GET: api/Reservations/5
        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Reservation))]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            return await _genericRepository.Query().Include(x => x.MeetingRoom).
                Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
        }


        // POST: api/Reservations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(int))]
        [SwaggerResponse(StatusCodes.Status409Conflict, Type = typeof(string))]
        public async Task<IActionResult> PostReservation(ReservationPostModel reserve)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<ReservationPostModel, ReservationUpdateModel>().ForMember(m => m.Id, opt => opt.NullSubstitute(0)));
            var mapper = new Mapper(config);
            var reserveMapped = mapper.Map<ReservationPostModel, ReservationUpdateModel>(reserve);
            if (await checkInitialTime(reserveMapped)) return Conflict("Choose appropriate time");
            if (await checkReserved(reserveMapped)) return Conflict("Time is taken");
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

            return Ok(reservation.Id);
        }

        // DELETE: api/Reservations/5
        [HttpDelete("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            await _genericRepository.Delete(id);

            return Ok();
        }

        [HttpPatch]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(int))]
        [SwaggerResponse(StatusCodes.Status409Conflict, Type = typeof(string))]
        public async Task<IActionResult> UpdateAsync(ReservationUpdateModel reserve)
        {
            if (reserve.Id == 0) return Conflict("Reservation with id 0 doesn't exist");
            if (await checkInitialTime(reserve)) return Conflict("Choose appropriate time");
            if (await checkReserved(reserve)) return Conflict("Time is taken");
            var user = await _userRepository.GetByIdAsync(reserve.UserId);
            var room = await _meetingRoomRepository.GetByIdAsync(reserve.MeetingRoomId);
            var reservation = new Reservation
            {
                Id = reserve.Id,
                User = user,
                MeetingRoom = room,
                TimeFrom = reserve.From,
                TimeTo = reserve.To
            };

            await _genericRepository.UpdateAsync(reservation);
            return Ok(reservation.Id);
        }
       
        private async Task<Boolean> checkInitialTime(ReservationUpdateModel reserve)
        {
            if (reserve.From < DateTime.Now || reserve.To < reserve.From || reserve.To.Subtract(reserve.From) > MaximumReservationTime)
                return true;

            return false;
        }

        private async Task<Boolean> checkReserved(ReservationUpdateModel reserve)
        {
            List<Reservation> res;
            if (reserve.Id == 0)
            {
                res = await _genericRepository.Query()
                    .Where(x => x.MeetingRoomId == reserve.MeetingRoomId && x.TimeFrom < reserve.To && x.TimeTo > reserve.From)
                    .ToListAsync();
            }
            else
            {
                res = await _genericRepository.Query()
                    .Where(x => x.MeetingRoomId == reserve.MeetingRoomId && x.TimeFrom < reserve.To && x.TimeTo > reserve.From && x.Id != reserve.Id)
                    .ToListAsync();
            }
            if (res.Any()) return true;

            return false;
        }
    }
}
