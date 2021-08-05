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
using System.Security.Cryptography;

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
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<User>))]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _genericRepository.GetAllAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(User))]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            return await _genericRepository.GetByIdAsync(id);
        }
        

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> PostUser(User user)
        {
            var users = await _genericRepository.Query().Where(x => x.Login == user.Login).ToListAsync();
            if (users.Any()) return Ok("User with this login already exists");
            user.Password = Hash(user.Password);
            await _genericRepository.AddAsync(user);

            return Ok(user.Id);
        }

        [HttpDelete]
        [SwaggerResponse(StatusCodes.Status409Conflict, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int id, string name, string login)
        {
            if (name == null && login == null)
            {
                await DeleteUser(id);
            }
            else if (id == 0 && login == null)
            {
                await DeleteUserByName(name);
            }
            else if (id == 0 && name == null)
            {
                await DeleteUserByLogin(login);
            }

            return Conflict("Not correct data given");
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
            if (userId == 0) return Conflict("No such user");
            await _genericRepository.Delete(userId);

            return Ok();
        }

        private async Task<IActionResult> DeleteUserByLogin(string login)
        {
            var userId = await _genericRepository.Query().Where(x => x.Login == login).Select(x => x.Id).FirstOrDefaultAsync();
            if (userId == 0) return Conflict("No such user");
            await _genericRepository.Delete(userId);

            return Ok();
        }

        [HttpPatch]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> UpdateUser(User user)
        {
            await _genericRepository.UpdateAsync(user);
            return Ok(user.Id);
        }

        [HttpGet("{login}, {password}")]
        [SwaggerResponse(StatusCodes.Status409Conflict, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(string))]
        public async Task<IActionResult> LoginUser(string login, string password)
        {
            var hashedPass = await _genericRepository.Query().Where(x => x.Login == login).Select(x => x.Password).FirstOrDefaultAsync();
            if (VerifyHashed(hashedPass, password)) return Ok("Successful");
            else return Conflict("Wrong login or password");
        }

        private static string Hash(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }

        private static bool VerifyHashed(string hashedPassword, string password)
        {
            byte[] buffer4;
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] src = Convert.FromBase64String(hashedPassword);
            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }
            return ByteArraysEqual(buffer3, buffer4);
        }

        private static bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            if (b1 == b2) return true;
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i]) return false;
            }
            return true;
        }
    }
}
