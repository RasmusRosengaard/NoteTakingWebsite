namespace webapi.DTOs
{
    public class AddUserToCanvasDto
    {
        public int UserId { get; set; }
        public string Role { get; set; } // "Owner", "Editor"
    }
}
