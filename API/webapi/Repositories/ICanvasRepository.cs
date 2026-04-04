using webapi.Models;
using webapi.DTOs;


namespace webapi.Repositories
{   
    public interface ICanvasRepository
    {
        Task<Canvas?> GetAsync(int canvasId, int userId);
        Task<IEnumerable<Canvas>> GetAllByUserIdAsync(int userId);
        Task AddAsync(Canvas canvas, int userId);
        Task DeleteCanvasAsync(int canvasId, int userId);
        Task SyncNotesAsync(int canvasId, int userId, List<NoteDto> incomingNotes);
        Task SaveChangesAsync();
    }
}