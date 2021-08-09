using BusinessLogic.DAL;
using System;

namespace BusinessLogic.Models
{
    public class Reservation : IId
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MeetingRoomId { get; set; }
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }

        public MeetingRoom MeetingRoom { get; set; }
        public User User { get; set; }

        public override bool Equals(object o)
        {
            return o is Reservation && ((Reservation)o).Id == this.Id && ((Reservation)o).UserId == this.UserId && ((Reservation)o).MeetingRoomId == this.MeetingRoomId && ((Reservation)o).TimeFrom == this.TimeFrom && ((Reservation)o).TimeTo == this.TimeTo && ((Reservation)o).User == this.User && ((Reservation)o).MeetingRoom == this.MeetingRoom;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
