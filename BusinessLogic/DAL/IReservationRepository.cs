using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.DAL
{
    public interface IReservationRepository : IGenericRepository<Reservation>
    {
        Task<List<Reservation>> GetInInterval(int roomId, DateTime from, DateTime to);
    }
}
