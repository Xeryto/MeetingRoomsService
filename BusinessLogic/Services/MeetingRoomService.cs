using BusinessLogic.DAL;
using BusinessLogic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class MeetingRoomService
    {
        private readonly IGenericRepository<MeetingRoom> _genericRepository;

        public MeetingRoomService(IGenericRepository<MeetingRoom> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<List<MeetingRoom>> Get()
        {
            return await _genericRepository.Query().ToListAsync();
        }

        public async Task<ActionResult<IEnumerable<MeetingRoom>>> GetAll()
        {
            return await Get();
        }

        public async Task<MeetingRoom> GetById(int id)
        {
            return await _genericRepository.Query().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<MeetingRoom> GetByName(string name)
        {
            return await _genericRepository.Query().Where(x => x.Name == name).FirstOrDefaultAsync();
        }

        public async Task<MeetingRoom> Add(MeetingRoom meetingRoom)
        {
            return await _genericRepository.AddAsync(meetingRoom);
        }

        public async Task<MeetingRoom> Delete(int id)
        {
            return await _genericRepository.Delete(id);
        }

        public async Task<MeetingRoom> Update(MeetingRoom meetingRoom)
        {
            return await _genericRepository.UpdateAsync(meetingRoom);
        }
    }
}
