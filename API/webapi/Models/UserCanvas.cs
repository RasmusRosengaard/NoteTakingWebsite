using webapi.Models;
namespace webapi.Models
{   
    public class UserCanvas
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public string Role { get; set; } 

        public int CanvasId { get; set; }
        public Canvas Canvas { get; set; }
    }
}