namespace webapi.Models
{
    public class Canvas
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Note> Notes { get; set; } = new List<Note>();

        public ICollection<UserCanvas> UserCanvases { get; set; } = new List<UserCanvas>();
    }
}
