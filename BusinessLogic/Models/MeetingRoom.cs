using BusinessLogic.DAL;

namespace BusinessLogic.Models
{
    public class MeetingRoom : IId
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
