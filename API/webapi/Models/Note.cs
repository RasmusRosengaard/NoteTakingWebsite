namespace webapi.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string? Content { get; set; }

        public float X { get; set; }
        public float Y { get; set; }

        public float Width { get; set; }  // new
        public float Height { get; set; } // new

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CanvasId { get; set; }
        public Canvas Canvas { get; set; }
    }
}
