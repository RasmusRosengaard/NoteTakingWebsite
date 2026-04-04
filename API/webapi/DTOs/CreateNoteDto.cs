namespace webapi.DTOs
{
    public class CreateNoteDto
    {
        public string? Content { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }  // new
        public float Height { get; set; } // new
    }
}