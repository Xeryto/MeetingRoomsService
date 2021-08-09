using BusinessLogic.DAL;

namespace BusinessLogic.Models
{
    public class User : IId
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
    }
}
