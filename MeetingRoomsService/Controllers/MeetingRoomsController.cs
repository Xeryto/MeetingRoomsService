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
    public class MeetingRoomsController : ControllerBase
    {
        private readonly IGenericRepository<MeetingRoom> _genericRepository;

        public MeetingRoomsController(IGenericRepository<MeetingRoom> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        // GET: api/MeetingRooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MeetingRoom>>> GetMeetingRooms()
        {
            return await _genericRepository.GetAllAsync();
        }

        // GET: api/MeetingRooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MeetingRoom>> GetMeetingRoom(int id)
        {
            return await _genericRepository.GetByIdAsync(id);
        }

        // POST: api/MeetingRooms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MeetingRoom>> PostMeetingRoom(MeetingRoom meetingRoom)
        {
            await _genericRepository.AddAsync(meetingRoom);

            return CreatedAtAction("GetMeetingRoom", new { id = meetingRoom.MeetingRoomID }, meetingRoom);
        }

        // DELETE: api/MeetingRooms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeetingRoom(int id)
        {
            await _genericRepository.Delete(id);

            return NoContent();
        }

        [HttpPatch]
        public async Task<ActionResult<MeetingRoom>> UpdateAsync(MeetingRoom meetingRoom)
        {
            await _genericRepository.UpdateAsync(meetingRoom);
            return CreatedAtAction("GetMeetingRoom", new { id = meetingRoom.MeetingRoomID }, meetingRoom);
        }
    }
}
