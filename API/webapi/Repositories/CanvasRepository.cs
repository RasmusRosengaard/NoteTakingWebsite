using webapi.Data;
using webapi.Models;
using Microsoft.EntityFrameworkCore;
using webapi.Repositories;
using webapi.DTOs;

public class CanvasRepository : ICanvasRepository
{
    private readonly AppDbContext _dbContext;

    public CanvasRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Load a canvas with notes for a specific user
    public async Task<Canvas?> GetAsync(int canvasId, int userId)
    {
        return await _dbContext.Canvases
            .Include(c => c.Notes)  // Include notes first
            .Where(c => c.Id == canvasId && c.UserCanvases.Any(uc => uc.UserId == userId))
            .FirstOrDefaultAsync();
    }

    // Load all canvases for a user
    public async Task<IEnumerable<Canvas>> GetAllByUserIdAsync(int userId)
    {
        return await _dbContext.UserCanvases
            .Where(uc => uc.UserId == userId)
            .Select(uc => uc.Canvas)
            .ToListAsync();
    }

    // Add a new canvas and link to user
    public async Task AddAsync(Canvas canvas, int userId)
    {
        await _dbContext.Canvases.AddAsync(canvas);

        var userCanvas = new UserCanvas
        {
            UserId = userId,
            Canvas = canvas,
            Role = "Owner"
        };

        await _dbContext.UserCanvases.AddAsync(userCanvas);
    }

    // Add a note to an existing canvas if the user has access
    public async Task<Note?> AddNoteAsync(int canvasId, int userId, Note note)
    {
        var hasAccess = await _dbContext.UserCanvases
            .AnyAsync(uc => uc.CanvasId == canvasId && uc.UserId == userId);

        if (!hasAccess) return null;

        note.CanvasId = canvasId;
        await _dbContext.Notes.AddAsync(note);

        return note;
    }

    public async Task<Note?> UpdateNoteAsync(int canvasId, int noteId, int userId, CreateNoteDto dto)
    {
        var note = await _dbContext.Notes
            .Include(n => n.Canvas)
            .Where(n => n.Id == noteId && n.Canvas.UserCanvases.Any(uc => uc.UserId == userId && uc.CanvasId == canvasId))
            .FirstOrDefaultAsync();

        if (note == null) return null;

        note.Content = dto.Content;
        note.X = dto.X;
        note.Y = dto.Y;
        note.Width = dto.Width;   // new
        note.Height = dto.Height; // new

        return note;
    }

    public async Task DeleteCanvasAsync(int canvasId, int userId)
    {
        var canvas = await _dbContext.Canvases
            .Where(c => c.Id == canvasId && c.UserCanvases.Any(uc => uc.UserId == userId))
            .FirstOrDefaultAsync();

        if (canvas != null)
        {
            _dbContext.Canvases.Remove(canvas);
        }
    }
    public async Task SyncNotesAsync(int canvasId, int userId, List<NoteDto> incomingNotes)
    {
        // 1. Verify access to canvas
        var canvas = await _dbContext.Canvases
            .Include(c => c.Notes)
            .Where(c => c.Id == canvasId && c.UserCanvases.Any(uc => uc.UserId == userId))
            .FirstOrDefaultAsync();

        if (canvas == null) return;

        // 2. Identify notes to delete (Notes in DB but not in the incoming list)
        var incomingIds = incomingNotes.Select(n => n.Id).Where(id => id > 0).ToList();
        var notesToRemove = canvas.Notes.Where(n => !incomingIds.Contains(n.Id)).ToList();
        _dbContext.Notes.RemoveRange(notesToRemove);

        // 3. Update or Add notes
        foreach (var dto in incomingNotes)
        {
            if (dto.Id > 0)
            {
                // Update existing
                var existingNote = canvas.Notes.FirstOrDefault(n => n.Id == dto.Id);
                if (existingNote != null)
                {
                    existingNote.Content = dto.Content;
                    existingNote.X = dto.X;
                    existingNote.Y = dto.Y;
                    existingNote.Width = dto.Width;
                    existingNote.Height = dto.Height;
                }
            }
            else
            {
                // Add new
                canvas.Notes.Add(new Note
                {
                    Content = dto.Content,
                    X = dto.X,
                    Y = dto.Y,
                    Width = dto.Width,
                    Height = dto.Height,
                    CanvasId = canvasId
                });
            }
        }
    }
    

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }


}