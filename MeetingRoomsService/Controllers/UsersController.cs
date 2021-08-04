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
    public class UsersController : ControllerBase
    {
        private readonly IGenericRepository<User> _genericRepository;

        public UsersController(IGenericRepository<User> genericRepository)
        {
            _genericRepository = genericRepository;
        }
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _genericRepository.GetAllAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            return await _genericRepository.GetByIdAsync(id);
        }
        

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostUser(User user)
        {
            await _genericRepository.AddAsync(user);

            return Ok(user.Id);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id, string name)
        {
            if (name == null)
            {
                await DeleteUser(id);
            }
            else if (id == 0)
            {
                await DeleteUserByName(name);
            }

            return Ok();
        }

        // DELETE: api/Users/5
        //[HttpDelete("{id}")]
        private async Task<IActionResult> DeleteUser(int id)
        {
            await _genericRepository.Delete(id);

            return Ok();
        }

        //[HttpDelete("{name}")]
        private async Task<IActionResult> DeleteUserByName(string name)
        {
            var userId = await _genericRepository.Query().Where(x => x.Name == name).Select(x => x.Id).FirstOrDefaultAsync();
            if (userId == 0) return Ok("No such user");
            await _genericRepository.Delete(userId);

            return Ok();
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateUser(User user)
        {
            await _genericRepository.UpdateAsync(user);
            return Ok(user.Id);
        }
    }
}
