using webapi.DTOs;

namespace webapi.DTOs
{
    public class CanvasSyncDto
    {
        public List<NoteDto> Notes { get; set; } = new();
    }
}
