using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingRoomsService.DAL
{
    public interface IGenericRepository<T> : IDisposable
    {
        public Task<ActionResult<IEnumerable<T>>> GetAllAsync();
        public Task<T> GetByIdAsync(object id);
        public Task<T> AddAsync(T entity);
        public Task<T> UpdateAsync(T entity);
        public Task<T> Delete(object id);
        public IQueryable<T> Query();
    }
}
