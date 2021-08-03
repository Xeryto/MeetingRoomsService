using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingRoomsService.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public int UserID { get; set; }
        public int MeetingRoomID { get; set; }
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }

        public MeetingRoom MeetingRoom { get; set; }
        public User User { get; set; }
    }
}
