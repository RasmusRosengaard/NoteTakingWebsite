using webapi.Models;
public interface INoteRepository
{
    Task<Note?> GetAsync(int id, int userId);
    Task AddAsync(Note note);
    Task SaveChangesAsync();
}