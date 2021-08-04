using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingRoomsService.Models
{
    public class ReservationUpdateModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MeetingRoomId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
