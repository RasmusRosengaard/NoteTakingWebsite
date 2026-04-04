namespace webapi.DTOs
{
    public class CanvasDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }

        public List<NoteDto> Notes { get; set; } = new();
    }
}