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

        public ReservationsController(IGenericRepository<Reservation> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        // GET: api/Reservations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            return await _genericRepository.GetAllAsync();
        }

        // GET: api/Reservations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            return await _genericRepository.GetByIdAsync(id);
        }

        // POST: api/Reservations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Reservation>> PostReservation(Reservation reservation)
        {
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

        [HttpPatch]
        public async Task<ActionResult<Reservation>> UpdateAsync(Reservation reservation)
        {
            await _genericRepository.UpdateAsync(reservation);
            return CreatedAtAction("GetReservation", new { id = reservation.ReservationId }, reservation);
        }
    }
}
