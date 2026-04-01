using webapi.Data;
using webapi.Models;
using Microsoft.EntityFrameworkCore;

public class NoteRepository : INoteRepository
{
    private readonly AppDbContext _dbContext; // <-- declare it

    public NoteRepository(AppDbContext dbContext)  // <-- inject via constructor
    {
        _dbContext = dbContext;
    }

    public async Task<Note?> GetAsync(int id, int userId)
    {
        // Example for many-to-many:
        return await _dbContext.UserNotes
            .Where(un => un.NoteId == id && un.UserId == userId)
            .Select(un => un.Note)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(Note note)
    {
        await _dbContext.Notes.AddAsync(note);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}