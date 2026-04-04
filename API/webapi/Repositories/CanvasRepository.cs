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

    public async Task<Canvas?> GetAsync(int canvasId, int userId)
    {
        return await _dbContext.Canvases
            .Include(c => c.Notes)
            .Where(c => c.Id == canvasId && c.UserCanvases.Any(uc => uc.UserId == userId))
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Canvas>> GetAllByUserIdAsync(int userId)
    {
        return await _dbContext.UserCanvases
            .Where(uc => uc.UserId == userId)
            .Select(uc => uc.Canvas)
            .ToListAsync();
    }

    public async Task AddAsync(Canvas canvas, int userId)
    {
        await _dbContext.Canvases.AddAsync(canvas);
        await _dbContext.UserCanvases.AddAsync(new UserCanvas
        {
            UserId = userId,
            Canvas = canvas,
            Role = "Owner"
        });
    }

    public async Task DeleteCanvasAsync(int canvasId, int userId)
    {
        var canvas = await _dbContext.Canvases
            .Where(c => c.Id == canvasId && c.UserCanvases.Any(uc => uc.UserId == userId))
            .FirstOrDefaultAsync();

        if (canvas != null) _dbContext.Canvases.Remove(canvas);
    }

    public async Task SyncNotesAsync(int canvasId, int userId, List<NoteDto> incomingNotes)
    {
        var canvas = await _dbContext.Canvases
            .Include(c => c.Notes)
            .Where(c => c.Id == canvasId && c.UserCanvases.Any(uc => uc.UserId == userId))
            .FirstOrDefaultAsync();

        if (canvas == null) return;

        // 1. Delete notes not present in the incoming list
        var incomingIds = incomingNotes.Select(n => n.Id).Where(id => id > 0).ToList();
        var notesToRemove = canvas.Notes.Where(n => !incomingIds.Contains(n.Id)).ToList();
        _dbContext.Notes.RemoveRange(notesToRemove);

        foreach (var dto in incomingNotes)
        {
            if (dto.Id > 0)
            {
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

    public async Task SaveChangesAsync() => await _dbContext.SaveChangesAsync();
}